@echo off
setlocal

:: Process the build type argument.
set buildTypeArg=%~1
if [%buildTypeArg%]==[/bd] (
    set buildType=BuildDebug
) else (
if [%buildTypeArg%]==[/br] (
    set buildType=BuildRelease
) else (
if [%buildTypeArg%]==[/bdp] (
    set buildType=BuildDebugAndPackage
) else (
if [%buildTypeArg%]==[/brp] (
    set buildType=BuildReleaseAndPackage
) else (
if [%buildTypeArg%]==[/brpp] (
    set buildType=BuildReleaseAndPackageAndPublish
) else (
    goto :invalidArguments
)
))))

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

    goto :visualStudioNotFound
)

:build
msbuild build\build.proj /t:%buildType% /nologo
exit /b %ERRORLEVEL%

:visualStudioNotFound
echo Error: build.cmd requires Visual Studio 2013 or 2015.
exit /b 1

:invalidArguments
echo Error: build.cmd requires a build type specified.
echo /bd - Build debug.
echo /br - Build release.
echo /bdp - Build debug and package via NuGet.
echo /brp - Build release and package via NuGet.
exit /b 2 