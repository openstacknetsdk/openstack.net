# Note: these values may only change during minor release
# Also, if the values change, assembly binding redirection will not work between the affected releases.

$Keys = @{
	'v3.5' = '8965cea5c205d3a3'
	'v4.0' = '8965cea5c205d3a3'
}

$KeysV2 = @{
	'net35' = '60ee1d542341ddcd'
	'net40' = '326b3de041f03d04'
	'net45' = '829455f81ec6a05d'
	'netcore45' = 'f190037ba32393f3'
	'portable-net40' = 'fcd35214427c5e62'
	'portable-net45' = 'd5a0cbda4b85df8a'
}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
