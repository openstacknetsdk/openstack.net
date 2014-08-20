# Note: these values may only change during minor release
$Keys = @{
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
