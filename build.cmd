@echo off
setlocal

:: Load the environment of the most recent version of Visual Studio installed
if not defined VisualStudioVersion (
    if defined VS140COMNTOOLS (
        call "%VS140COMNTOOLS%\VsDevCmd.bat"
        goto :build
    )

    if defined VS120COMNTOOLS (
        call "%VS120COMNTOOLS%\VsDevCmd.bat"
        goto :build
    )

    echo Error: build.cmd requires Visual Studio 2013 or 2015.
    exit /b 1
)

:build
msbuild build\build.proj /nologo
exit /b %ERRORLEVEL%
