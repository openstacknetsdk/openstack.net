# Note: these values may only change during minor release
$Keys = @{
	'v3.5' = '8965cea5c205d3a3'
	'v4.0' = '8965cea5c205d3a3'
}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
