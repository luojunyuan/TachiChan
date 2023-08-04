param (
    [string]$csprojPath
)

$xml = New-Object XML
$xml.Load($csprojPath)

$targetFramework = $xml.SelectSingleNode('//Project/PropertyGroup/TargetFramework')

# $targetFramework = $xml.Project.PropertyGroup.TargetFramework
if ($targetFramework -ne $null)
{
    $targetFramework.'#text' = 'net472'
}
else
{
    Write-Host "not found 'TargetFramework'"
}

$xml.Save($csprojPath)
