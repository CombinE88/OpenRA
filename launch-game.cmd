@echo off
title OpenRA
for %%x in (%*) do (
  if "%%~x" EQU "Game.Mod" (goto launch)
)

:launchmod
OpenRA.Game.exe Game.Mod=bam
goto end
:launch
OpenRA.Game.exe %*

:end
if %errorlevel% neq 0 goto crashdialog
exit /b
:crashdialog
echo ----------------------------------------
echo OpenRA has encountered a fatal error.
echo   * Log Files are available in Documents\OpenRA\Logs
echo   * FAQ is available at https://github.com/OpenRA/OpenRA/wiki/FAQ
echo ----------------------------------------
pause