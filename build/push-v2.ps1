. .\version.ps1

If ($VersionV2.EndsWith('-dev')) {
	$host.ui.WriteErrorLine("Cannot push development version '$VersionV2' to NuGet.")
	Exit 1
}

..\src\.nuget\NuGet.exe 'push' ".\nuget\OpenStack.Net.$VersionV2.nupkg"
