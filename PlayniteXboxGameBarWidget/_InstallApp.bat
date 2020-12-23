:: Playnite XboxGameBar Widget
@echo off
title Playnite XboxGameBar Widget Installer

:: Set playnitePath
set playnitePath=%USERPROFILE%\AppData\Local\Playnite\Playnite.FullscreenApp.exe
set param1=
set param2=

:: Check if the bat is being run as admin
net session >nul 2>&1
if not %errorlevel% == 0 (
    set ErrorMessage=No elevated privileges! Please run this program as Administrator
	goto Error
)

:choice
set /P c=Skip library update on Playnite launch from the widget? [Y/N]
if /I "%c%" equ "Y" set param1=--nolibupdate

set /P c=Skip splash image on Playnite launch from the widget? [Y/N]
if /I "%c%" equ "Y"  set param2=--hidesplashscreen

:: Update Registry
cls & echo Updating Registry...
reg add HKEY_CLASSES_ROOT\PlayniteFullscreen /t REG_SZ  /d  "URL:PlayniteFullscreen" /f >nul
reg add HKEY_CLASSES_ROOT\PlayniteFullscreen /v "URL Protocol" /t  REG_SZ  /d  "" /f >nul
reg add HKEY_CLASSES_ROOT\PlayniteFullscreen\DefaultIcon /t  REG_SZ  /d  %playnitePath%  /f >nul
reg add HKEY_CLASSES_ROOT\PlayniteFullscreen\shell\open\command /t  REG_SZ  /d "%playnitePath% %param1% %param2%" /f >nul

:: Register App
echo Registering UWP App...
powershell add-apppackage -register %~dp0appxmanifest.xml

:: Installation Completed
echo. & echo Installation Completed Successfully! & echo.
pause

:: Exit
:Exit
exit

:: Show error message
:Error
color 4b
cls
echo ERROR: %ErrorMessage%
pause > nul