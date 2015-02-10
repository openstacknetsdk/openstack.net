param (
	[switch]$Debug,
	[string]$VisualStudioVersion = "12.0",
	[switch]$NoDocs,
	[string]$Verbosity = "normal",
	[string]$Logger
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

If ($Version.Contains('-')) {
	$KeyConfiguration = 'Dev'
} Else {
	$KeyConfiguration = 'Final'
}

If ($VersionV2.Contains('-')) {
	$KeyConfigurationV2 = 'Dev'
} Else {
	$KeyConfigurationV2 = 'Final'
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

# Attempt to restore packages up to 3 times, to improve resiliency to connection timeouts and access denied errors.
$maxAttempts = 3
For ($attempt = 0; $attempt -lt $maxAttempts; $attempt++) {
	&$nuget 'restore' $SolutionPath
	If ($?) {
		Break
	} ElseIf (($attempt + 1) -eq $maxAttempts) {
		$host.ui.WriteErrorLine('Failed to restore required NuGet packages, aborting!')
		exit $LASTEXITCODE
	}
}

If ($Logger) {
	$LoggerArgument = "/logger:$Logger"
}

&$msbuild '/nologo' '/m' '/nr:false' '/t:rebuild' $LoggerArgument "/verbosity:$Verbosity" "/p:Configuration=$SolutionBuildConfig" "/p:Platform=Mixed Platforms" "/p:VisualStudioVersion=$VisualStudioVersion" "/p:KeyConfiguration=$KeyConfiguration" "/p:KeyConfigurationV2=$KeyConfigurationV2" $SolutionPath
if (-not $?) {
	$host.ui.WriteErrorLine('Build failed, aborting!')
	exit $LASTEXITCODE
}

# By default, do not create a NuGet package unless the expected strong name key files were used
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

if (-not (Test-Path 'nuget')) {
	mkdir "nuget"
}

# The NuGet packages reference XML documentation which is post-processed by SHFB. If the -NoDocs flag is specified,
# these files are not created so packaging will fail.
If (-not $NoDocs) {
	&$nuget 'pack' '..\src\corelib\corelib.nuspec' '-OutputDirectory' 'nuget' '-Prop' "Configuration=$BuildConfig" '-Version' "$Version" '-Symbols'
	&$nuget 'pack' '..\src\OpenStack.Net\OpenStack.Net.V2.nuspec' '-OutputDirectory' 'nuget' '-Prop' "Configuration=$BuildConfig" '-Version' "$VersionV2" '-Symbols'
}
