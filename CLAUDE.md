# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DoubleClickFix is a C# .NET 9 Windows Forms application that intercepts and filters erroneous mouse double-clicks (hardware "chattering") via a low-level `WH_MOUSE_LL` mouse hook. It also supports drag-and-drop stabilization and mouse wheel bounce filtering. Distributed via Microsoft Store (MSIX) and GitHub Releases (standalone ZIP).

## Build Commands

```bash
# Build main application
msbuild DoubleClickFix/DoubleClickFix.csproj /p:Configuration=Release /p:Platform=AnyCPU

# Or with dotnet CLI
dotnet build DoubleClickFix/DoubleClickFix.csproj --configuration Release

# Run tests
dotnet test DoubleClickFix.Tests/DoubleClickFix.Tests.csproj

# Run a single test
dotnet test DoubleClickFix.Tests/DoubleClickFix.Tests.csproj --filter "FullyQualifiedName~TestLeftClickIgnored"
```

**Important:** Always keep C# file encodings as UTF-8 with BOM (code page 65001) when editing.

## Creating Releases

**GitHub release (standalone ZIP):**
```bash
git tag -a v1.x.x.x
git push origin v1.x.x.x
```
This triggers the GitHub Action that builds and publishes the release. Add release notes on GitHub afterward.

**Microsoft Store package:** Use Visual Studio → Publish → Create App Packages (builds `.msixbundle` for x86/x64/arm64). Upload via Partner Portal.

## Architecture

### Solution Structure

- **`DoubleClickFix/`** – Main WinForms app (core logic + UI)
- **`DoubleClickFix.Tests/`** – xUnit tests with mock infrastructure
- **`DoubleClickFix.Package/`** – MSIX packaging for the Store
- **`DoubleClickFix.Benchmarks/`** – BenchmarkDotNet benchmarks
- **`Directory.Build.props`** – Shared version number (currently 1.4.13.0)

### Core Components and Data Flow

```
Program.cs (entry point)
  ├─ Detects Store vs Standalone → chooses settings implementation
  ├─ Enforces single instance (mutex)
  ├─ Creates MouseHook + SystemEventsHandler
  ├─ Installs WH_MOUSE_LL hook
  └─ Runs InteractiveForm (system tray UI)

MouseHook.cs (core filtering logic)
  └─ HookCallback: intercepts all mouse events system-wide
       ├─ Double-click filtering: suppresses DOWN if elapsed since last UP < threshold
       ├─ Drag-lock: enters drag-lock mode after >5px movement, suppresses jitter UP/DOWN events
       └─ Wheel bounce: suppresses opposite-direction wheel events within threshold

SystemEventsHandler.cs
  └─ Uninstalls/reinstalls hook on session lock, power suspend/resume, exceptions

Settings (ISettings interface)
  ├─ StandaloneSettings → HKEY_CURRENT_USER\Software\DoubleClickFix\v1
  └─ StoreSettings → Windows.Storage.ApplicationDataContainer
```

### Key Files

- **`MouseHook.cs`** – All filtering logic; the most important file
- **`NativeMethods.cs` / `INativeMethods.cs`** – P/Invoke wrappers (user32.dll, kernel32.dll)
- **`Program.cs`** – Bootstrap, dependency wiring, single-instance enforcement
- **`InteractiveForm.cs`** – Settings UI, tray icon, logs display
- **`SettingsBase.cs`** – Abstract settings with defaults and change notification

### Testing Infrastructure

Tests use mock implementations in `DoubleClickFix.Tests/Helper/`:
- `TestNativeMethods.cs` – Tracks hook calls and suppressed events
- `TestSettings.cs` – In-memory settings
- `TestLogger.cs` – Captures log output
- `HookStruct.cs` – Helper to build `MSLLHOOKSTRUCT` values for test events

### Command-Line Arguments

- **`-nohook`** – Skip installing the mouse hook (auto-applied in Debug mode)
- **`-interactive` / `-i`** – Show UI on startup (auto-applied in Debug mode)

### Language Override (for testing)

Set `languageOverride` in `DoubleClickFix/app.config` to: `en`, `de`, `es`, `fr`, or `it`.
