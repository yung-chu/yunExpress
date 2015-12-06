@echo off
cd /d %~dp0Build
cmd /c "build.bat" & cd .. & pause & exit /b

