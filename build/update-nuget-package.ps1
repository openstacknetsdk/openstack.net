param (
  [string]$pkgPath,
  [string]$docXmlPath
)

$pkg = [System.IO.Compression.ZipFile]::Open($pkgPath, "Update" )
[System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($pkg, $docXmlPath, "lib\net45\openstacknet.xml", "Optimal")
$pkg.Dispose()