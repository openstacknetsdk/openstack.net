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
$nuget = '..\src\.nuget\NuGet.exe'

if ($VisualStudioVersion -eq '4.0') {
	$msbuild = "$env:windir\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
} Else {
	$msbuild = "${env:ProgramFiles(x86)}\MSBuild\$VisualStudioVersion\Bin\MSBuild.exe"
}

&$nuget 'restore' $SolutionPath
&$msbuild '/nologo' '/m' '/nr:false' '/t:rebuild' "/p:Configuration=$SolutionBuildConfig" "/p:Platform=Mixed Platforms" "/p:VisualStudioVersion=$VisualStudioVersion" $SolutionPath
if (-not $?) {
	$host.ui.WriteErrorLine('Build failed, aborting!')
	exit $LASTEXITCODE
}

# By default, do not create a NuGet package unless the expected strong name key files were used
if (-not $SkipKeyCheck) {
	. .\keys.ps1

	foreach ($pair in $Keys.GetEnumerator()) {
		$assembly = Resolve-FullPath -Path "..\src\corelib\bin\$($pair.Key)\$BuildConfig\openstacknet.dll"
		# Run the actual check in a separate process or the current process will keep the assembly file locked
		powershell -Command ".\check-key.ps1 -Assembly '$assembly' -ExpectedKey '$($pair.Value)' -Build '$($pair.Key)'"
		if (-not $?) {
			$host.ui.WriteErrorLine("Failed to verify strong name key for build $($pair.Key).")
			Exit $LASTEXITCODE
		}
	}

	foreach ($pair in $KeysV2.GetEnumerator()) {
		$assembly = Resolve-FullPath -Path "..\src\OpenStack.Net\bin\$($pair.Key)\$BuildConfig\OpenStack.Net.dll"
		# Run the actual check in a separate process or the current process will keep the assembly file locked
		powershell -Command ".\check-key.ps1 -Assembly '$assembly' -ExpectedKey '$($pair.Value)' -Build '$($pair.Key)'"
		if (-not $?) {
			$host.ui.WriteErrorLine("Failed to verify strong name key for build $($pair.Key).")
			Exit $LASTEXITCODE
		}
	}
}

if (-not (Test-Path 'nuget')) {
	mkdir "nuget"
}

&$nuget 'pack' '..\src\corelib\corelib.nuspec' '-OutputDirectory' 'nuget' '-Prop' "Configuration=$BuildConfig" '-Version' "$Version" '-Symbols'
&$nuget 'pack' '..\src\OpenStack.Net\OpenStack.Net.V2.nuspec' '-OutputDirectory' 'nuget' '-Prop' "Configuration=$BuildConfig" '-Version' "$VersionV2" '-Symbols'
