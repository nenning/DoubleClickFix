# Plan: optional "reset threshold on chatter" setting

## Problem

`MouseHook` suppresses a chattering click by comparing the new Down's
timestamp against `previousUpTime[button]` — but that timestamp is only
updated on a *genuine* (forwarded) Up (`MouseHook.cs:382`). During a
suppressed burst it never moves, so every bounce in the burst is measured
against the timestamp of the very first click's release, not against the
previous bounce.

Effect: the whole burst must fit inside one `threshold`-ms window anchored to
the first Up. If a mouse's bounces are spread out further than that (e.g. a
late third contact bounce), that bounce falls outside the window, gets
treated as a genuine new click, and is forwarded — reproducing the reported
"logged as ignored, but still double-clicks" symptom.

This plan adds an **optional** setting that instead resets ("slides") the
window on every bounce, so a burst of any length is suppressed as long as
consecutive gaps stay under the threshold — at the cost of no longer being
able to tell where a legitimate fast double-click starts once such a burst
is underway.

## Behavior change

- **Off (default, current behavior)**: anchor stays fixed at the last
  genuine Up. Preserves existing test expectations
  (`TestSustainedChatteringAllSuppressed`, `TestMultipleClicks`, etc.).
- **On**: `previousUpTime[button]` is updated on *every* Up for that button —
  suppressed or genuine — not just genuine ones. Each bounce's release
  becomes the new anchor for the next Down comparison.

Example with the setting on, `LeftThreshold = 50`:

```
D1 (t=0)    real            → forwarded
U1 (t=10)   real            → forwarded, previousUpTime = 10
D2 (t=40)   Δ=30 < 50       → ignored, suppressNextUp = {Left}
U2 (t=45)   suppressed      → previousUpTime = 45   (new: was skipped before)
D3 (t=90)   Δ=45 < 50       → ignored (would have been Δ=80, allowed, if anchor stayed at 10)
U3 (t=95)   suppressed      → previousUpTime = 95
```

Without the setting, D3 at t=90 is 80ms past the original Up (t=10) and
would be forwarded as a genuine second click — the exact failure mode from
the bug report. With the setting on, D3 is compared against the last
bounce's Up (t=45) instead, so it's still caught.

Trade-off to call out to the user: while a chatter burst keeps recurring
within `threshold` ms of the previous bounce, no click can ever get through
— so a real, deliberate double-click performed while chatter is still
"active" would also be swallowed. This is inherent to the ask ("filter
everything except the first click in the interval") and should be
documented as an advanced/opt-in setting, not the default.

## Scope

One global boolean, not per-button — it changes *how* the anchor is
tracked, not the per-button threshold values themselves. Applies uniformly
to Left/Right/Middle/X1/X2 handling in `HandleMouseButton`.

Not applicable to the wheel-bounce path (`HandleWheel`) or drag-lock path —
both already have their own independent suppression models; out of scope
for this change.

## Implementation steps

1. **`ISettings.cs`** — add `bool IsChatterResetEnabled { get; set; }`
   (name TBD, see Open Questions).

2. **`SettingsBase.cs`** — mirror the existing `IsRemoteDesktopDetectionEnabled`
   pattern (int-backed bool, default 0/off):
   - `protected int chatterResetEnabled = 0;`
   - public bool property with `FireSettingsChanged()` on change
   - reset to `0` in `Reset()`

3. **`StandaloneSettings.cs`** — persist via
   `SaveSetting(key, chatterResetEnabled)` / `LoadSetting(key, chatterResetEnabled)`,
   same as `remoteDesktopDetection` (registry `REG_DWORD`).

4. **`StoreSettings.cs`** — persist via `SaveSetting(chatterResetEnabled)` /
   `LoadSetting(chatterResetEnabled)`, same pattern as `remoteDesktopDetection`.

5. **`MouseHook.cs`** — in `HandleMouseButton`'s `buttonUp` branch
   (currently `MouseHook.cs:375-383`):
   ```csharp
   } else if (buttonUp) {
       if (suppressNextUp.Remove(activeButton)) {
           currentlyDownButtons.Remove(activeButton);
           if (settings.IsChatterResetEnabled) {
               previousUpTime[activeButton] = hookStruct.time;
           }
           return IgnoreMouseEvent;
       }
       // genuine up
       currentlyDownButtons.Remove(activeButton);
       previousUpTime[activeButton] = hookStruct.time;
   }
   ```
   No changes needed in the `buttonDown` branch — the comparison logic
   (`delta < threshold`) already reads from `previousUpTime`, it just needs
   that dictionary to be kept current.

6. **UI (`InteractiveForm.cs` / `.Designer.cs` / `.resx`)** — add a checkbox
   next to the existing threshold controls, following the
   `remoteDesktopCheckBox` wiring exactly:
   - Designer: new `CheckBox`, `CheckedChanged += OnChatterResetCheckBoxChanged`
   - `InteractiveForm.cs`: load initial state in the two places
     `remoteDesktopCheckBox.Checked` is set (constructor-time and
     settings-changed refresh), add
     `OnChatterResetCheckBoxChanged(sender, e) => settings.IsChatterResetEnabled = chatterResetCheckBox.Checked;`
   - Add a short explanatory label/tooltip string to `Resources.resx`
     (English only for the initial PR; other 20 language `.resx` files can
     fall back to English until translated — consistent with how new
     strings have been rolled out before).

## Tests to add (`MouseHookTests.cs`)

1. `TestChatterBeyondThresholdNotSuppressed` — setting **off** (default):
   documents the existing limitation. One genuine click, one in-window
   bounce, then a bounce that lands just past `threshold` ms from the
   *original* Up → asserted **allowed** (current, unchanged behavior).

2. `TestChatterResetExtendsSuppressionWindow` — setting **on**: same event
   timeline as test 1, but with `IsChatterResetEnabled = true` → the late
   bounce is now asserted **ignored**, because it's within `threshold` ms of
   the *previous* bounce's Up rather than the original one.

3. `TestChatterResetDoesNotAffectGenuineSecondClick` — setting **on**: a
   genuine click, a gap larger than `threshold` with no chatter in between,
   then a second genuine click → still **allowed**, to confirm the setting
   only changes anchor tracking, not the threshold comparison itself.

4. Optional: a `SettingsBaseTests.cs` round-trip test (save/load) for the
   new persisted field, matching existing coverage for
   `IsRemoteDesktopDetectionEnabled`.

## Open questions before implementing

- **Setting name / label wording** — options: `IsChatterResetEnabled`,
  `ResetThresholdOnChatter`, `ExtendedChatterSuppression`. Needs a
  user-facing label too (English string for the checkbox + a one-line
  tooltip explaining the trade-off).
- **Default state** — plan assumes **off** for backward compatibility.
  Confirm that's desired rather than making it the new default behavior.
- **Where in the UI** — main settings panel next to the per-button
  thresholds, or tucked into an "advanced" section given the trade-off
  (can swallow a genuine fast double-click during an active burst)?
- **Unbounded burst duration** — with the setting on, a burst has no
  maximum total length (it only ends when a gap exceeds `threshold`). Is
  that acceptable, or should there be a hard cap (e.g. give up and forward
  after N suppressed pairs, or after some absolute maximum elapsed time)?
