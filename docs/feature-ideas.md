Filtering depth
- Independent horizontal vs. vertical wheel threshold — currently shared; some worn wheels only chatter in one axis.
- Double-move / cursor-jitter filtering — analogous to the existing drag-lock, but for stationary micro-jitter (some failing mice send spurious tiny deltas at rest), which could cause unwanted click-drags in precision work (e.g., CAD, image editing).
- maybe with the RDP on a separate advanced settings dialog to not clutter the UI?
- Double-click/drag-lock state isn't per-device: previousUpTime, currentlyDownButtons, isDragLocked, etc. (MouseHook.cs:69-81) are keyed only by MouseButtons, not by device handle. With two mice plugged in, releasing a button on one can suppress a legitimate click on the other within the threshold window.
- Keyboard chattering fix — a sibling WH_KEYBOARD_LL hook applying the same debounce pattern to worn/bouncing keys. Full design: [plan-keyboard-chattering-fix.md](plan-keyboard-chattering-fix.md).
- Optional "reset threshold on chatter" setting — slides the suppression anchor forward on every Up (not just genuine ones) so a multi-bounce burst longer than one threshold window is still fully suppressed, at the cost of possibly swallowing a genuine fast double-click during an active burst. Full design: [plan-reset-threshold-on-chatter.md](plan-reset-threshold-on-chatter.md).


Architecture/testability
- InteractiveForm.cs is a 750+ line God-object (UI wiring, update-check HTTP calls, toast notifications, language/theme management). Update-checking and language handling are natural extraction candidates.
- SystemEventsHandler couples directly to static Microsoft.Win32.SystemEvents with no interface — zero test coverage, unlike everything else which follows the INativeMethods-style mockable pattern.
- StandaloneSettings/StoreSettings duplicate near-identical save/load boilerplate per field; a shared IPropertyStore (Get<T>/Set<T>) would cut this down significantly.
- knownDeviceTypes/knownDevicePaths (MouseHook.cs:62,64) grow unbounded for the process lifetime — every new raw-input device handle adds an entry that's never evicted (e.g. a Bluetooth mouse gets a fresh handle each reconnect).
- Dark-mode/theme calls (NativeMethods.IsDarkMode/ApplyDarkTitleBar, NativeMethods.cs:314,330) are called directly as statics from Program.cs, bypassing the INativeMethods mockable pattern the rest of the code follows — theme logic has no test path.
- TestNativeMethods.GetWindowLong/SetWindowLong throw NotImplementedException (TestNativeMethods.cs:74,76) — a future InteractiveForm test touching DPI/WS_EX_TOOLWINDOW code will crash instead of no-op.
- TestNativeMethods.TryProcessRawInput can only simulate Mouse or TouchPad, never TouchScreen — the touchscreen bypass path (MouseHook.cs:196) has zero test coverage despite mock scaffolding existing for the other two device types.
- hookHandle is read without hookLock inside HookCallback while Install()/Uninstall() mutate it under the lock (MouseHook.cs:136-164); since SystemEventsHandler calls Uninstall() from session-lock/power-suspend events, there's a low-severity race between the hook thread and the resume thread.


Won't Do
- IsCurrentDeviceIgnored() (MouseHook.cs:124) treats an unidentifiable device handle (0) as always ignored, silently disabling filtering with no user-visible signal — could confuse a user after a driver change makes a device briefly unclassifiable.
- GetXButton's if/else on the XBUTTON1/XBUTTON2 bitmask (MouseHook.cs:428-431) looks like it could silently drop X2 if both bits were set, but Windows never sets both bits in a single WM_XBUTTONDOWN/UP message — not a live bug.