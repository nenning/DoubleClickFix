# Plan: keyboard chattering fix (WH_KEYBOARD_LL)

## Problem

The same contact-bounce hardware degradation that causes mice to
double-click also affects keyboard switches: a worn or dirty key can send
a spurious extra make/break (`WM_KEYDOWN`/`WM_KEYUP`) pair, producing a
duplicated character. `MouseHook.cs` already solves this for mouse
buttons with a per-button "ignore Down if it follows the matching Up too
quickly" filter; this plan adds a sibling `KeyboardHook.cs` using
`WH_KEYBOARD_LL` and the same suppression pattern.

## Decisions locked in

These were discussed and decided before writing this plan:

1. **Single global threshold**, not per-key. Adds one `KeyThreshold`
   setting and one new "Keyboard" entry in the existing
   `mouseButtonComboBox` dropdown, reusing the same slider +
   enabled-checkbox UI mouse buttons already use. A sparse "capture the
   bad key" list would be more precise but needs new list-management UI;
   out of scope for v1 — can be revisited if a global threshold proves too
   blunt in practice (see Open Questions).
2. **Modifier/lock keys excluded by default**, with an explicit opt-in
   checkbox ("Include modifier keys"), unchecked by default. Modifiers
   (Shift/Ctrl/Alt/Win/CapsLock/NumLock/ScrollLock) are frequently held
   for chorded shortcuts; suppressing a bounce mid-chord is more
   disruptive than a stray character, so they're never touched unless the
   user explicitly opts in.
3. **Off by default** (`KeyThreshold = -1`). Matches every existing
   threshold except `LeftThreshold`. Avoids suppressing legitimate fast
   same-key typing (e.g. double letters) for anyone who hasn't
   deliberately enabled it.
4. **Per-device ignore reused.** `KeyboardHook` classifies devices via raw
   input the same way `MouseHook` does and exposes its own
   `CurrentDevicePath`, feeding the *same* `ISettings.IgnoredDevicePaths`
   set mouse devices already use (device paths are globally unique, so no
   collision risk). Lets a user exclude one keyboard (e.g. a known-good
   external one) while still guarding another (e.g. a failing laptop
   built-in keyboard), or vice versa.
5. **The native hook is only installed while the feature is enabled**
   (`KeyThreshold >= 0`), unlike `MouseHook`, which is always installed
   whenever `UseHook` is true. Rationale: `WH_KEYBOARD_LL` is the same
   primitive keyloggers use and is a much more sensitive thing to have
   silently running system-wide than a mouse hook, especially since this
   feature defaults to off. `MouseHook` can get away with always-on
   because its core purpose (left-click debounce) is on by default; the
   keyboard fix is opt-in only, so there's no reason to intercept every
   keystroke on the machine until the user has actually turned it on.
   Raw input *registration* (for `CurrentDevicePath`/ignore-list tracking)
   stays unconditional, same as the mouse — it's passive notification,
   not interception, and needs to work even before the feature is
   enabled so the user can identify/ignore a device ahead of time.

## Design

### Native additions — `NativeMethods.cs` / `INativeMethods.cs`

```csharp
private const int WH_KEYBOARD_LL = 13;

internal const nint WM_KEYDOWN = 0x0100;
internal const nint WM_KEYUP = 0x0101;
internal const nint WM_SYSKEYDOWN = 0x0104; // sent instead of WM_KEYDOWN while Alt is held
internal const nint WM_SYSKEYUP = 0x0105;

[StructLayout(LayoutKind.Sequential)]
internal struct KBDLLHOOKSTRUCT {
    public uint vkCode;
    public uint scanCode;
    public uint flags;
    public uint time;
    public nint dwExtraInfo;
}

internal delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

public nint SetKeyboardHook(LowLevelKeyboardProc lpfn) {
    var hhk = SetWindowsHookEx(WH_KEYBOARD_LL, lpfn, nint.Zero, 0);
    return hhk == nint.Zero ? throw new Win32Exception(Marshal.GetLastWin32Error()) : hhk;
}
```

`UnhookWindowsHook`/`CallNextHook` are already generic over `nint` and can
be reused as-is (mirrors how `SetHook` is the only mouse-specific
overload today).

`LLKHF_INJECTED = 0x10` is the keyboard equivalent of the mouse's
`LLMHF_INJECTED = 0x1` — used identically, for on-screen-keyboard /
RDP-injected passthrough (see below). Note the bit value differs from the
mouse constant; do not share the `LLMHF_INJECTED` constant between hooks.

Raw input registration (in the existing `RegisterForRawInput`): add one
more `RAWINPUTDEVICE` entry with `UsagePage = HID_USAGE_PAGE_GENERIC
(0x01)`, `Usage = HID_USAGE_GENERIC_KEYBOARD (0x06)`, same
`RIDEV_INPUTSINK` flag as the others.

`TryGetRawInputDeviceHandle`/`TryGetDevicePath` are already
type-agnostic (they only read `RAWINPUTHEADER`) and need no changes.
`TryProcessRawInput`'s `DeviceType` enum (`Mouse`/`TouchScreen`/`TouchPad`)
is **not** needed for keyboards — unlike touchpads, there's no ambiguous
"physical device that might actually be synthetic" case to disambiguate;
an on-screen keyboard always sends its input via `SendInput`, which the
low-level hook already reports via `LLKHF_INJECTED`. So `KeyboardHook`
only needs device *path* tracking for the ignore-list feature, not device
*type* classification.

### Settings — `ISettings.cs` / `SettingsBase.cs` / `StandaloneSettings.cs` / `StoreSettings.cs`

- `int KeyThreshold { get; set; }` — mirrors `LeftThreshold`, default `-1`.
- `bool IsModifierKeySuppressionEnabled { get; set; }` — int-backed bool,
  mirrors `IsRemoteDesktopDetectionEnabled`, default `false`.
- No new ignore-list setting — reuse `IgnoredDevicePaths` /
  `AddIgnoredDevice` / `RemoveIgnoredDevice` as-is.
- `Reset()` gets `KeyThreshold = -1;` and
  `IsModifierKeySuppressionEnabled = false;` added.
- Persist both new fields the same way `remoteDesktopDetection` already is
  in each settings implementation (registry `REG_DWORD` for
  `StandaloneSettings`, `ApplicationDataContainer` value for
  `StoreSettings`).

### `KeyboardHook.cs` (new file, sibling to `MouseHook.cs`)

State, mirroring `MouseHook`'s per-button dictionaries but keyed by
`vkCode` (`int`) instead of a fixed 5-entry `MouseButtons` enum, since the
key space is ~150 values and sparse:

```csharp
private readonly Dictionary<int, uint> previousUpTime = [];
private readonly HashSet<int> currentlyDownKeys = [];
private readonly HashSet<int> suppressNextUp = [];
```

Plus the same `knownDeviceTypes`-style device-path tracking as
`MouseHook` (minus `DeviceType`), `CurrentDevicePath`,
`CurrentDeviceChanged`, `isCurrentDeviceIgnored`, `isRemoteSession`.

**Gating the native hook on the feature being enabled** (decision #5).
`MouseHook.Install()` only checks `settings.UseHook`; `KeyboardHook.Install()`
adds a second gate:

```csharp
private bool IsFeatureEnabled => settings.KeyThreshold >= 0;

public bool Install() {
    lock (hookLock) {
        if (!IsFeatureEnabled) {
            return false; // not an error — feature is simply off
        }
        if (settings.UseHook && hookHandle == nint.Zero) {
            try {
                keyboardProc = HookCallback;
                hookHandle = nativeMethods.SetKeyboardHook(keyboardProc);
            } catch (Win32Exception ex) {
                logger.Log($"{Resources.HookNotInstalled}: {ex.Message}");
                return false;
            }
        }
        return hookHandle != nint.Zero;
    }
}
```

Unlike the mouse, this hook's install state must react *live* to the
setting changing — not just at startup — since the user can flip
`KeyThreshold` between `-1` and a real value at any time via the
dropdown/slider without restarting the app. `SettingsChanged()` (already
called via `settings.RegisterSettingsChangedListener`, same as
`MouseHook`) drives this:

```csharp
private void SettingsChanged() {
    isCurrentDeviceIgnored = IsCurrentDeviceIgnored();
    if (IsFeatureEnabled) {
        Install();
    } else {
        Uninstall();
    }
}
```

Both `Install()` and `Uninstall()` are already idempotent (guarded by
`hookHandle == nint.Zero` / `!= nint.Zero`), so this fires safely on
every settings change, not just ones that touch `KeyThreshold` — same
tolerance `MouseHook.SettingsChanged()` already relies on today.

`Program.cs` needs no special-casing as a result: it constructs
`KeyboardHook` and calls `Install()` unconditionally alongside
`mouseHook.Install()`, exactly like the mouse — `Install()` itself
becomes a documented no-op when the feature is off. One difference:
`Program.cs` shows a red "No mouse hook installed!" banner when
`mouseHook.Install()` returns `false`, because that always indicates a
real failure. For `KeyboardHook`, `Install()` returning `false` is the
*expected* steady state (feature disabled) — do **not** wire up an
equivalent banner keyed off its return value; a genuine install failure
while the feature is enabled is already logged inside `Install()`'s
`catch` block, which is sufficient for a secondary/opt-in feature.

Modifier/lock exclusion set (static, checked when
`!settings.IsModifierKeySuppressionEnabled`):

```csharp
private static readonly FrozenSet<int> ModifierVkCodes = new HashSet<int> {
    0x10, 0x11, 0x12,       // VK_SHIFT, VK_CONTROL, VK_MENU (Alt)
    0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, // L/R variants of the above
    0x5B, 0x5C,             // VK_LWIN, VK_RWIN
    0x14, 0x90, 0x91,       // VK_CAPITAL, VK_NUMLOCK, VK_SCROLL
}.ToFrozenSet();
```

`HookCallback` dispatch:

```csharp
internal nint HookCallback(int nCode, nint wParam, nint lParam) {
    try {
        return HandleKey(nCode, wParam, lParam);
    } catch (Exception ex) {
        try { logger.Log($"Error in hook callback: {ex}"); } catch { }
        return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
    }
}
```

`HandleKey` logic (mirrors `HandleMouseButton`):

1. Early out (forward unfiltered) when: `nCode < 0`, `KeyThreshold < 0`,
   current device is ignored, or `wParam` isn't one of
   `WM_KEYDOWN`/`WM_KEYUP`/`WM_SYSKEYDOWN`/`WM_SYSKEYUP`. `WM_SYSKEY*`
   **must** be handled, not just `WM_KEY*` — Windows sends the `SYS`
   variant instead of the normal one while Alt is held (e.g. Alt+letter
   combos), and a chattering key held with Alt down would otherwise
   bypass the hook entirely.
2. Marshal `KBDLLHOOKSTRUCT`. If `(flags & LLKHF_INJECTED) != 0 &&
   (!isRemoteSession || settings.IsRemoteDesktopDetectionEnabled)` →
   forward (on-screen keyboard / RDP passthrough, same rule as mouse).
3. `int vk = (int)hookStruct.vkCode;` If
   `!settings.IsModifierKeySuppressionEnabled && ModifierVkCodes.Contains(vk)`
   → forward unconditionally.
4. Determine `keyDown = wParam is WM_KEYDOWN or WM_SYSKEYDOWN`.
5. **Auto-repeat guard** — this is the one case with no mouse analogue:
   while a key is held, Windows re-sends `WM_KEYDOWN` repeatedly with no
   intervening `WM_KEYUP`. If `keyDown && currentlyDownKeys.Contains(vk)`,
   this is an OS repeat, not a new press — forward immediately, skip the
   threshold check entirely. Only evaluate the threshold on the
   transition into "down" (`vk` not already in `currentlyDownKeys`).
6. Down/Up bookkeeping — identical structure to
   `HandleMouseButton`'s `buttonDown`/`buttonUp` branches:
   `uint diff = unchecked(hookStruct.time - previousUpTime.GetValueOrDefault(vk, 0)); long delta = diff;`
   `ignore = delta < settings.KeyThreshold` (no `MinDelay`/`belowMin`
   concept needed — that only exists on the mouse side for the "0ms means
   suppress everything" edge case doc'd elsewhere; can add later if
   requested). On ignore: `suppressNextUp.Add(vk)`, log, return
   `IgnoreKeyEvent` (`const nint IgnoreKeyEvent = 1;`, same convention as
   mouse). On Up: if `suppressNextUp.Remove(vk)` → suppress the matching
   Up too (prevents an orphaned release), else record
   `previousUpTime[vk] = hookStruct.time`.
7. No drag-lock or wheel-bounce equivalents — those are mouse-specific
   and out of scope here.

`ProcessRawInput`/`RegisterForRawInput` methods mirror `MouseHook`'s
almost exactly, minus the `DeviceType`/`isCurrentDeviceTouchDevice`
tracking (see Native additions above).

### `Program.cs` / `SystemEventsHandler.cs` wiring

- `Program.cs`: construct `using KeyboardHook keyboardHook = new(settings, logger, new NativeMethods());`
  next to `mouseHook`; call `keyboardHook.RegisterForRawInput(rawInputWindow.Handle)`
  and `keyboardHook.Install()` alongside the mouse hook's; route
  `rawInputWindow`'s `WM_INPUT` to both hooks' `ProcessRawInput` (each
  hook only reacts to raw input from its own device class, since a
  keyboard's raw input header type differs from a mouse's — no
  cross-talk).
- `SystemEventsHandler.cs`: currently hard-coded to a single `MouseHook`.
  Simplest change matching this codebase's "don't build abstractions for
  two things" style (see `feature-ideas.md`'s existing note about this
  class): add a second constructor parameter `KeyboardHook keyboardHook`
  and duplicate the `Install()`/`Uninstall()`/`RefreshRemoteSessionState()`
  calls in each handler (`OnSessionEnding`, `OnSessionSwitch`,
  `OnPowerModeChanged`, `SetupExceptionHandlers`'s crash-recovery paths).
  Not worth extracting an `IInputHook` interface for two implementations.

### UI — `InteractiveForm.cs` / `.Designer.cs` / `.resx`

- Add `Resources.Keyboard` string; append it as a 7th item to
  `mouseButtonComboBox.Items` (after Wheel, index 6).
- `OnSelectedMouseButtonChanged` / `UpdateSettings`: add `case 6:` reading
  and writing `settings.KeyThreshold`.
- New "Include modifier keys" checkbox, wired the same way as
  `remoteDesktopCheckBox` → `OnRemoteDesktopCheckBoxChanged`:
  `settings.IsModifierKeySuppressionEnabled = includeModifierKeysCheckBox.Checked;`.
  Consider disabling/graying this checkbox unless the combo box's
  current selection is "Keyboard" (or just always show it — TBD, see
  Open Questions).
- New "currently active keyboard" label + "Ignore this device" checkbox,
  parallel to `currentDeviceLabel` / `ignoreCurrentDeviceCheckBox`, but
  bound to `keyboardHook.CurrentDevicePath` /
  `keyboardHook.CurrentDeviceChanged` instead of `mouseHook`'s. Needs its
  own `UpdateIgnoreKeyboardDeviceControls()` mirroring
  `UpdateIgnoreDeviceControls()`.
- English-only strings for the initial PR, consistent with how new
  strings have been rolled out before (other language `.resx` files fall
  back to English until translated).

## Performance, security, and false-positive (keylogger) considerations

### Performance

Not expected to be a concern, but the discipline below is load-bearing,
not optional — `WH_KEYBOARD_LL` runs synchronously on the thread that
installed it for *every* keystroke system-wide, and Windows can silently
drop/mark-unresponsive a hook that blocks past the `LowLevelHooksTimeout`
(~300ms default). Keystrokes are far less frequent than mouse-move
events and the per-event work here (`Dictionary<int,_>`/`HashSet<int>`
lookups keyed by `vkCode`) is O(1), so raw cost isn't the risk — a stall
or uncaught exception in the callback is, because it freezes typing
*machine-wide*, not just in this app. Requirements carried over from
`MouseHook.HookCallback`'s existing pattern, not new work:
- `HookCallback` wraps `HandleKey` in try/catch and always falls through
  to `CallNextHook`, exactly as shown in the Design section above.
- No blocking calls (I/O, `Marshal.AllocHGlobal` loops, etc.) inside
  `HandleKey` — the existing `Dictionary`/`HashSet` operations are the
  only per-event state touched.
- Already covered process-wide by `Program.cs`'s existing
  `ProcessPriorityClass.AboveNormal` bump — no keyboard-specific change
  needed there.

### Security

This hook observes every keystroke on the machine, not just ones
directed at this app — including passwords typed into other windows.
Hard constraints for the implementation, not just style preferences:
- `HandleKey` may read `vkCode`/`scanCode`/`flags`/`time` for suppression
  bookkeeping **only**. Never call character-translation APIs
  (`ToUnicode`, `GetKeyNameText`, etc.) to reconstruct typed text, and
  never persist or transmit raw per-keystroke data anywhere — not the log
  pane, not `WriteCrashLog`, not any future telemetry. Logging *which key*
  was suppressed (a `Keys` enum name, e.g. "Left Shift") is fine and
  mirrors the mouse's `buttonTextLookup`; logging *what was typed* is not.
- A stuck suppression bug is higher-stakes here than on the mouse:
  silently eating a keystroke while someone is typing a password (no
  visual feedback) is worse and harder to notice than an eaten click.
  `suppressNextUp` must not be able to leak — covered by the auto-repeat
  and suppression tests below, but worth an explicit sanity pass during
  implementation, not just relying on the listed unit tests.
- No action needed re: UAC/secure desktop — Windows' UIPI isolates the
  secure desktop (Ctrl+Alt+Del, UAC consent prompt) from any user-mode
  hook regardless of this app's code, so those keystrokes are
  architecturally unreachable.
- Decision #5 (hook only installed while the feature is enabled) doubles
  as a security control: it minimizes how many machines ever have this
  hook active at all, not just a UX nicety.

### Will this get flagged as a keylogger / blocked by AV?

Realistic risk, not hypothetical — `SetWindowsHookEx(WH_KEYBOARD_LL, ...)`
is a well-known behavioral signature some AV/EDR heuristics flag
regardless of what the code does with the captured data, and this has
affected other legitimate low-level-keyboard-hook tools (e.g.
AutoHotkey-style utilities) before. Mitigations, in order of leverage:
- **Already in place**: standalone releases are Authenticode-signed via
  SignPath (see `readme.md`'s Code Signing Policy section) — the single
  highest-leverage mitigation for AV/SmartScreen reputation. Signing
  doesn't grant immunity from behavioral heuristics, but it establishes a
  verifiable publisher identity and builds reputation over time.
- **Already in place**: the repo is open source, so the "no keystroke
  content is read/stored/transmitted" claim is independently verifiable
  against the actual `KeyboardHook.cs` source, not just asserted.
- **New, do as part of this feature**: add a short "Keyboard Hook &
  Privacy" section to `readme.md`, mirroring the existing "Compatibility
  with Anti-Cheat Software" section, explicitly stating what the hook
  does and does not do (suppresses chattering key events by timing only;
  never reads, stores, or transmits typed content) and linking to
  `KeyboardHook.cs`. Do this before/alongside shipping the feature, not
  as an afterthought once a report comes in.
- **If it happens anyway**: false-positive flags on legitimate
  `WH_KEYBOARD_LL` tools are a known, recoverable situation, not a
  dead end — submit to Microsoft Defender
  (`https://www.microsoft.com/wdsi/filesubmission`) and other vendors as
  needed. Can't be fully preempted, only mitigated by the points above.

## Scope

In scope: `WM_KEYDOWN`/`WM_KEYUP`/`WM_SYSKEYDOWN`/`WM_SYSKEYUP` filtering
via one global threshold, modifier exclusion (with opt-in), per-keyboard
ignore list, RDP/injected passthrough matching the mouse's existing rule.

Out of scope for this plan (candidates for follow-up):
- Per-key thresholds / sparse "capture this key" list (decision #1 above).
- Any keyboard equivalent of drag-lock or wheel-bounce — there's no
  keyboard analogue.
- `MinDelay`-style "0ms threshold still suppresses" edge case — add only
  if it turns out to matter for keyboards the way it does for mice.

## Implementation steps

1. `ISettings.cs` — add `KeyThreshold`, `IsModifierKeySuppressionEnabled`.
2. `SettingsBase.cs` — backing fields, properties with
   `FireSettingsChanged()`, `Reset()` entries.
3. `StandaloneSettings.cs` — registry persistence for both fields.
4. `StoreSettings.cs` — `ApplicationDataContainer` persistence for both fields.
5. `NativeMethods.cs` / `INativeMethods.cs` — `WH_KEYBOARD_LL` constant,
   `KBDLLHOOKSTRUCT`, `LowLevelKeyboardProc`, `SetKeyboardHook`,
   `WM_KEYDOWN`/`WM_KEYUP`/`WM_SYSKEYDOWN`/`WM_SYSKEYUP`, `LLKHF_INJECTED`,
   keyboard `RAWINPUTDEVICE` registration entry.
6. `KeyboardHook.cs` — new file, structure described above.
7. `Program.cs` — construct, register raw input, install/uninstall alongside `mouseHook`.
8. `SystemEventsHandler.cs` — accept and drive a second hook.
9. `InteractiveForm.cs` / `.Designer.cs` / `Resources.resx` — dropdown entry,
   modifier checkbox, per-device ignore row.
10. `readme.md` — add a "Keyboard Hook & Privacy" section mirroring the
    existing "Compatibility with Anti-Cheat Software" section (see
    Performance/Security/False-positive considerations above).

## Tests to add (`KeyboardHookTests.cs`, new file mirroring `MouseHookTests.cs`)

Needs a `TestNativeMethods` extension (add `SetKeyboardHook` tracking,
mirroring the existing `SetHook`/`SetHookCounter` pair) and a
`KeyHookStruct` test helper mirroring `HookStruct` but for
`KBDLLHOOKSTRUCT`.

1. `TestChatteringKeyIgnored` — two `WM_KEYDOWN`/`WM_KEYUP` pairs for the
   same `vkCode` within `KeyThreshold` ms → second Down and its matching
   Up both suppressed (return `1`), `CallNextHookCounter` unchanged for
   that pair.
2. `TestGenuineRetypeAllowed` — same, but gap exceeds `KeyThreshold` →
   both events forwarded.
3. `TestAutoRepeatNeverSuppressed` — held key: Down, then several more
   Downs with no interleaved Up, all within a few ms of each other →
   every repeat Down forwarded (regression test for the auto-repeat
   guard — this is the case a naive port of `HandleMouseButton` would
   get wrong).
4. `TestModifierKeyNeverSuppressedByDefault` — rapid Down/Up/Down for
   `VK_SHIFT` within threshold, `IsModifierKeySuppressionEnabled = false`
   → both Downs forwarded.
5. `TestModifierKeySuppressedWhenOptedIn` — same as above with
   `IsModifierKeySuppressionEnabled = true` → second Down suppressed like
   any other key.
6. `TestSysKeyVariantsSuppressed` — repeat test 1 using
   `WM_SYSKEYDOWN`/`WM_SYSKEYUP` instead of the plain variants → same
   suppression behavior (regression test for the Alt-held case).
7. `TestKeyThresholdDisabledPassesEverythingThrough` — `KeyThreshold = -1`
   (default) → rapid repeats of the same key are never suppressed.
8. `TestIgnoredKeyboardDeviceBypassesFiltering` — device path in
   `IgnoredDevicePaths` → chattering key on that device forwarded
   unfiltered, matching `MouseHookTests`' existing ignored-device
   coverage.
9. Optional `SettingsBaseTests.cs` round-trip tests for the two new
   fields, matching existing `IsRemoteDesktopDetectionEnabled` coverage.
10. `TestInstallNoOpWhenFeatureDisabled` — `KeyThreshold = -1` (default),
    call `Install()` → returns `false`, `TestNativeMethods.SetHookCounter`
    stays `0` (native `SetWindowsHookEx` never called).
11. `TestEnablingThresholdInstallsHookLive` — start with `KeyThreshold = -1`,
    then set `settings.KeyThreshold = 50` at runtime (no restart) →
    `SetHookCounter` becomes `1`, confirming `SettingsChanged()` installs
    reactively.
12. `TestDisablingThresholdUninstallsHookLive` — start with
    `KeyThreshold = 50` and the hook installed, then set
    `settings.KeyThreshold = -1` → `UnhookWindowsHookCounter` becomes `1`.

## Open questions before implementing

- **Modifier checkbox visibility/enablement** — always visible, or only
  enabled/relevant while "Keyboard" is selected in the dropdown? The
  dropdown's existing slider+checkbox pair is per-selected-item, but this
  new checkbox is keyboard-only global state, so it doesn't fit the same
  per-item show/hide pattern the other five entries use.
- **`SystemEventsHandler` constructor churn** — adding a required second
  parameter changes an existing public constructor signature; confirm
  that's fine (it's `internal`, only used from `Program.cs`, so should be
  safe) rather than making it optional/nullable.
- **Where to surface "this key was suppressed" in the log** — mouse logs
  include the button name (`Resources.Left` etc. via
  `buttonTextLookup`). For ~150 possible keys, decide between logging the
  raw `vkCode`, a `Keys` enum `.ToString()` (WinForms already has this
  mapping via `System.Windows.Forms.Keys`), or a curated friendly-name
  table like the mouse's — `Keys` enum `.ToString()` is the obvious
  zero-effort default.
- **Interaction with OS key-repeat delay/rate settings** — the auto-repeat
  guard (step 5 in the `HandleKey` logic) assumes any Down while the key
  is already tracked as down is a repeat, regardless of Windows'
  configured repeat rate. This should be correct in all cases since a
  genuine bounce always involves an intervening Up, but worth confirming
  during manual testing with fast repeat-rate settings.
