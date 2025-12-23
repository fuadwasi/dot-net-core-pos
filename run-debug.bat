@echo off
echo Starting POS System...
echo.
cd /d "%~dp0"
"src\POSSystem.Maui\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\POSSystem.Maui.exe"
echo.
echo Application exited with code: %ERRORLEVEL%
pause
