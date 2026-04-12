@echo off
setlocal enabledelayedexpansion

echo.
echo DoubleClickFix Updater
echo ======================
echo.

:: Find the running DoubleClickFix.exe process and its path
set "EXE_PATH="
for /f "usebackq tokens=*" %%A in (`wmic process where "name='DoubleClickFix.exe'" get ExecutablePath /value 2^>nul`) do (
    set "LINE=%%A"
    if "!LINE:ExecutablePath=!" neq "!LINE!" for /f "tokens=1,* delims==" %%X in ("%%A") do set "EXE_PATH=%%Y"
)

if not defined EXE_PATH goto :not_running

:: Remove trailing carriage return from wmic output
for /f "delims=" %%P in ("!EXE_PATH!") do set "EXE_PATH=%%P"
for %%F in ("!EXE_PATH!") do set "INSTALL_DIR=%%~dpF"

echo Found DoubleClickFix at: !EXE_PATH!
echo Install directory: !INSTALL_DIR!
echo Update from: %~dp0
echo.

:: Check if source and target directories are the same
if /i "!INSTALL_DIR!"=="%~dp0" goto :same_dir

:: Kill the running process
echo Stopping DoubleClickFix...
taskkill /f /im DoubleClickFix.exe >nul 2>&1
timeout /t 2 /nobreak >nul

:: Copy files from this directory to the install directory
echo.
echo Copying files...
echo.
robocopy "%~dp0." "!INSTALL_DIR!." /E /IS /IT /XF update.bat
set "ROBOCOPY_EXIT=!errorlevel!"

if !ROBOCOPY_EXIT! GTR 7 goto :copy_failed

:: Restart the application
echo.
echo Starting DoubleClickFix...
start "" "!INSTALL_DIR!DoubleClickFix.exe"

echo.
echo DoubleClickFix updated successfully.
echo.
pause
exit /b 0

:not_running
echo Could not update: DoubleClickFix is not running.
echo.
echo Please start DoubleClickFix first, then run this updater.
echo.
pause
exit /b 1

:same_dir
echo Could not update: The updater is running from the install directory.
echo.
echo Please extract the new release to a different folder and run update.bat from there.
echo.
pause
exit /b 1

:copy_failed
echo.
echo Error: Failed to copy files. Robocopy exit code: !ROBOCOPY_EXIT!
echo   Source: %~dp0
echo   Target: !INSTALL_DIR!
echo.
pause
exit /b 1
