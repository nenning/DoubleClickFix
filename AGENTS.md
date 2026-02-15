# Gemini Context File for DoubleClickFix

This document provides context for the Gemini CLI to understand the DoubleClickFix project.

## Project Overview

DoubleClickFix is a C# .NET Windows Forms application designed to intercept and filter erroneous mouse double-clicks. It is distributed through the Microsoft Store as an MSIX package. The project uses low-level mouse hooks to function.

## Project Structure

The solution (`DoubleClickFix.slnx`) contains the following key projects:

-   **`DoubleClickFix/`**: The main Windows Forms application project (`.csproj`). This contains the core logic for the mouse hook and the user interface.
-   **`DoubleClickFix.Package/`**: A Windows Application Packaging Project (`.wapproj`) used to build the MSIX package for distribution via the Microsoft Store.
-   **`DoubleClickFix.Tests/`**: A unit testing project for the main application.

## Build and Packaging

The application is built using MSBuild or Visual Studio.

-   **Building the Application**: The main application can be built by targeting the `DoubleClickFix.csproj` file.
-   **Building the Store Package**: To create the distributable package for the Microsoft Store, the `DoubleClickFix.Package.wapproj` project must be built.

### Platform Architecture

The project is configured to build for `x86`, `x64`, `arm64` architecture for the Microsoft Store. The `DoubleClickFix.Package.wapproj` is configured to create an `AppxBundle`.
For normal solution builds and standalone releases `AnyCPU` shall be used.

### Key Build Commands

msbuild DoubleClickFix/DoubleClickFix.csproj /p:Configuration=Release /p:Platform=AnyCPU

## Native Dependencies

The application uses P/Invoke to call native Windows APIs for the low-level mouse hook (`user32.dll`, `kernel32.dll`). The relevant files are:
- `DoubleClickFix/NativeMethods.cs`
- `DoubleClickFix/INativeMethods.cs`
- `DoubleClickFix/MouseHook.cs`

The use of `IntPtr` or `nint` ensures that the native calls are compatible across different system architectures.

## Other topics

Make absolutely sure to keep the file encodings as they are when doing edits! C# files are always UTF-8 with signature (code page 65001)!