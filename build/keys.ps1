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

If ($VersionV2.Contains('-')) {

	# Use the development keys
	$KeysV2 = @{
		'net35' = 'adcdb5032b31c0fa'
		'net40' = 'adcdb5032b31c0fa'
		'net45' = 'adcdb5032b31c0fa'
		'netcore45' = 'adcdb5032b31c0fa'
		'portable-net40' = 'adcdb5032b31c0fa'
		'portable-net45' = 'adcdb5032b31c0fa'
	}

} Else {

	# Use the final release keys
	$KeysV2 = @{
		'net35' = 'e6ea8e3f398d7b2e'
		'net40' = 'e6ea8e3f398d7b2e'
		'net45' = 'e6ea8e3f398d7b2e'
		'netcore45' = 'e6ea8e3f398d7b2e'
		'portable-net40' = 'e6ea8e3f398d7b2e'
		'portable-net45' = 'e6ea8e3f398d7b2e'
	}

}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
