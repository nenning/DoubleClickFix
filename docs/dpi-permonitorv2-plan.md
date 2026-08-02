# Plan: PerMonitorV2 via container-based layout

**Goal:** Replace absolute resx positioning in `InteractiveForm` with `TableLayoutPanel`/`Dock`/`AutoSize` layout so WinForms can rescale the form reliably on runtime DPI changes, then re-enable `PerMonitorV2`.

## Background

- The app currently runs with `ApplicationHighDpiMode` = `SystemAware` (see `DoubleClickFix.csproj`): the layout is computed once at startup DPI and bitmap-stretched on DPI changes. Slightly blurry after a DPI switch, but the layout never breaks.
- `PerMonitorV2` was tried and reverted: WinForms' runtime rescaling (`WM_DPICHANGED`) is unreliable for absolutely-positioned, anchored controls loaded from resx. Switching 200% → 100% mangled the layout (clipped combos, collapsed textboxes).
- The 20 language resx files (`InteractiveForm.*.resx`) contain **strings only** — all layout data (`Location`/`Size`/`Anchor`) lives solely in the base `InteractiveForm.resx`, so localization is unaffected by a layout rework.

## Current structure (what must be preserved)

- Header: `label1` (app description) left, `themeComboBox` + `languageComboBox` top-right
- `groupBoxDevice`: current device label + "Ignore this device" checkbox
- `groupBox1` "Per Mouse Button": button combo + enable checkbox, slider + threshold textbox, 0-ms-gap checkbox, drag-fix checkbox + start/end delay fields
- `groupBoxGeneral`: RDP checkbox, run-at-startup checkbox, Reset button
- `groupBox2` (description text) and `groupBox3` (log) side by side, growing with window height
- `groupBox4` (right column, test area): pictureBox + richTextBox + 6 hover-toggled test checkboxes — the only part needing overlay-style placement
- `bottomPanel`: GitHub link, update link, version label

## Step 1 — Build the new layout skeleton (in `InitializeComponent`, hand-written)

Root: one `TableLayoutPanel` (`Dock=Fill`), **2 columns** (main = 100%, test column = AutoSize) with rows:

| Row | Content | Sizing |
|-----|---------|--------|
| 0 | header panel: `label1` (fill) + theme/language combos | AutoSize |
| 1 | `groupBoxDevice` | AutoSize |
| 2 | `groupBox1` | AutoSize |
| 3 | `groupBoxGeneral` | AutoSize |
| 4 | inner 2-col TLP: `groupBox2` (40%) / `groupBox3` (60%) | 100% (fills rest) |
| 5 | `bottomPanel` (spans both columns) | AutoSize |

`groupBox4` occupies column 1, spanning rows 0–4.

Inside each group box: a nested `TableLayoutPanel` with `Dock=Fill`, `AutoSize` rows; labels/checkboxes `AutoSize=true`; textboxes with fixed *character-based* widths (set via `Width = LogicalToDeviceUnits(...)` or TLP Percent columns); `logTextBox`/`descriptionTextBox`/`richTextBox1` `Dock=Fill`. `Padding`/`Margin` instead of coordinate gaps.

Test checkboxes in `groupBox4`: put them in a small `FlowLayoutPanel` (still hover-toggled via existing `OnShowTestControls`/`OnHideTestControls`) instead of scattered absolute positions.

## Step 2 — Purge layout data from base resx

Remove all `*.Location`, `*.Size`, `*.Anchor`, `*.AutoSize`, `$this.ClientSize` entries from `InteractiveForm.resx`; keep `Text`, `ToolTip`, `Items`, icons. `resources.ApplyResources(...)` calls stay — they then apply strings only. Language resx files: untouched. Set form `MinimumSize`/initial `ClientSize` in code using DPI-relative logic (`AutoScaleDimensions` + `AutoScaleMode.Font` stays).

## Step 3 — Reconcile code-behind

- `ClampToWorkingArea`, `RestartBounds`, `-bounds` restart args: verify they still behave when the form's size is partly AutoSize-driven
- `OnShown` re-clamp workaround and the 125% clipping fix (#36): likely removable afterwards — retest before deleting
- Hover show/hide of test controls, `updateLinkLabel.Visible`, tray show/hide: unchanged logic, just re-verify

## Step 4 — Re-enable PerMonitorV2

Flip `ApplicationHighDpiMode` back to `PerMonitorV2` in the csproj (remove the SystemAware comment). `ApplicationConfiguration.Initialize()` already picks it up.

## Step 5 — Test matrix

- Static: launch at 100 / 125 / 150 / 200% — no clipping, no overlap
- Dynamic: change scale while running (both directions, incl. 200 → 100), drag between mixed-DPI monitors, DPI change while minimized to tray, then restore
- Long-text languages (de, ru, bn) — AutoSize should handle what absolute layout couldn't
- Language/theme change restart with `-bounds` across a DPI change
- Regression: tooltips, tab order, dark/light theme rendering of TLP backgrounds

## Risks / notes

- Designer support: hand-written TLP layout means the VS WinForms designer becomes read-mostly for this form — acceptable since the form rarely changes
- `TrackBar` inside TLP sometimes reports odd preferred sizes → give its cell an explicit Percent width
- One PR, but commit Step 1+2 (layout swap, still SystemAware) separately from Step 4 (PMv2 flip) so a regression can be bisected

Estimated effort: the layout swap (Steps 1–2) is the bulk; Steps 3–5 are verification-heavy.
