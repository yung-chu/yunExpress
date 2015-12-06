@echo off
cd /d %~dp0build
cmd /c "build.bat /target:Database" & cd .. & pause & exit /b
