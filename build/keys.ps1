# Note: these values may only change during major release
# Also, if the values change, assembly binding redirection will not work between the affected releases.

If ($Version.Contains('-')) {

	# Use the development keys
	$Keys = @{
		'v3.5' = 'a7b9d24dda86d33e'
		'v4.0' = 'a7b9d24dda86d33e'
	}

} Else {

	# Use the final release keys
	$Keys = @{
		'v3.5' = '8965cea5c205d3a3'
		'v4.0' = '8965cea5c205d3a3'
	}

}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
