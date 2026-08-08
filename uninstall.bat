@echo off
setlocal enabledelayedexpansion

echo.
echo DoubleClickFix Uninstaller
echo ==========================
echo.

:: Stop the running application, if any
tasklist /fi "imagename eq DoubleClickFix.exe" 2>nul | find /i "DoubleClickFix.exe" >nul
if !errorlevel! equ 0 (
    echo Stopping DoubleClickFix...
    taskkill /f /im DoubleClickFix.exe >nul 2>&1
    timeout /t 2 /nobreak >nul
)

:: Remove the "run at Windows startup" registry entry, if present
echo Removing startup entry...
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Run" /v DoubleClickFix /f >nul 2>&1

:: Remove application settings from the registry
echo Removing settings...
reg delete "HKCU\Software\DoubleClickFix" /f >nul 2>&1

:: Remove the crash log, if present
if exist "%LocalAppData%\DoubleClickFix" (
    echo Removing crash log...
    rmdir /s /q "%LocalAppData%\DoubleClickFix" >nul 2>&1
)

echo.
echo DoubleClickFix has been uninstalled.
echo You can now delete this folder to remove the remaining program files.
echo.
pause
exit /b 0
