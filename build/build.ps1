param (
	[switch]$Debug,
	[string]$VisualStudioVersion = "12.0",
	[switch]$SkipKeyCheck,
	[switch]$NoDocs
)

# build the solution
$SolutionPath = "..\src\openstack.net.sln"

# make sure the script was run from the expected path
if (!(Test-Path $SolutionPath)) {
	$host.ui.WriteErrorLine('The script was run from an invalid working directory.')
	exit 1
}

. .\version.ps1

If ($Debug) {
	$BuildConfig = 'Debug'
} Else {
	$BuildConfig = 'Release'
}

If ($NoDocs -and -not $Debug) {
	$SolutionBuildConfig = $BuildConfig + 'NoDocs'
} Else {
	$SolutionBuildConfig = $BuildConfig
}

# build the main project
$msbuild = "$env:windir\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"

&$msbuild '/nologo' '/m' '/nr:false' '/t:rebuild' "/p:Configuration=$SolutionBuildConfig" "/p:Platform=Mixed Platforms" "/p:VisualStudioVersion=$VisualStudioVersion" $SolutionPath
if ($LASTEXITCODE -ne 0) {
	$host.ui.WriteErrorLine('Build failed, aborting!')
	exit $p.ExitCode
}

# By default, do not create a NuGet package unless the expected strong name key files were used
if (-not $SkipKeyCheck) {
	. .\keys.ps1

	foreach ($pair in $Keys.GetEnumerator()) {
		$assembly = Resolve-FullPath -Path "..\src\corelib\bin\$($pair.Key)\$BuildConfig\openstacknet.dll"
		# Run the actual check in a separate process or the current process will keep the assembly file locked
		powershell -Command ".\check-key.ps1 -Assembly '$assembly' -ExpectedKey '$($pair.Value)' -Build '$($pair.Key)'"
		if ($LASTEXITCODE -ne 0) {
			Exit $p.ExitCode
		}
	}
}

if (-not (Test-Path 'nuget')) {
	mkdir "nuget"
}

..\src\.nuget\NuGet.exe 'pack' '..\src\corelib\corelib.nuspec' '-OutputDirectory' 'nuget' '-Prop' "Configuration=$BuildConfig" '-Version' "$Version" '-Symbols'
