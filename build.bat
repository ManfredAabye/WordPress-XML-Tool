@echo off
REM Build the project using dotnet csproj file
dotnet build WordPress_XML_Tool.csproj -c Release
REM Check if the build was successful
if %errorlevel% neq 0 (
    echo Build failed. Exiting.
    REM exit /b %errorlevel%
)

pause
