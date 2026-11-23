param(
    [Parameter(Mandatory = $true)]
    [string] $DepsPath,
    [Parameter(Mandatory = $true)]
    [string] $InterfaceAssemblyPath
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $DepsPath)) {
    throw "Dependency manifest not found: $DepsPath"
}
if (-not (Test-Path -LiteralPath $InterfaceAssemblyPath)) {
    throw "UVCS issue-tracker API assembly not found: $InterfaceAssemblyPath"
}

$manifest = Get-Content -LiteralPath $DepsPath -Raw | ConvertFrom-Json
$interfaceVersion = [Reflection.AssemblyName]::GetAssemblyName($InterfaceAssemblyPath).Version.ToString()
$target = $manifest.targets.PSObject.Properties[$manifest.runtimeTarget.name].Value
$pluginDll = [IO.Path]::GetFileNameWithoutExtension(
    [IO.Path]::GetFileNameWithoutExtension($DepsPath)
) + '.dll'
$plugin = $target.PSObject.Properties |
    Where-Object { $_.Value.runtime.PSObject.Properties.Name -contains $pluginDll } |
    Select-Object -First 1

if ($null -eq $plugin) {
    throw "Plugin runtime entry '$pluginDll' not found in $DepsPath"
}

$plugin.Value.dependencies | Add-Member -MemberType NoteProperty -Name 'issuetrackerinterface' -Value '1.0.0' -Force
$plugin.Value.dependencies | Add-Member -MemberType NoteProperty -Name "issuetrackerinterface.Reference" -Value $interfaceVersion -Force

$apiRuntime = [PSCustomObject]@{
    runtime = [PSCustomObject]@{
        'issuetrackerinterface.dll' = [PSCustomObject]@{}
    }
}
$referenceRuntime = [PSCustomObject]@{
    runtime = [PSCustomObject]@{
        'issuetrackerinterface.dll' = [PSCustomObject]@{
            assemblyVersion = $interfaceVersion
            fileVersion = $interfaceVersion
        }
    }
}
$target | Add-Member -MemberType NoteProperty -Name 'issuetrackerinterface/1.0.0' -Value $apiRuntime -Force
$target | Add-Member -MemberType NoteProperty -Name "issuetrackerinterface.Reference/$interfaceVersion" -Value $referenceRuntime -Force
$manifest.libraries | Add-Member -MemberType NoteProperty -Name 'issuetrackerinterface/1.0.0' -Value ([PSCustomObject]@{
    type = 'project'
    serviceable = $false
    sha512 = ''
}) -Force
$manifest.libraries | Add-Member -MemberType NoteProperty -Name "issuetrackerinterface.Reference/$interfaceVersion" -Value ([PSCustomObject]@{
    type = 'reference'
    serviceable = $false
    sha512 = ''
}) -Force

$json = ConvertTo-Json -InputObject $manifest -Depth 100
[IO.File]::WriteAllText($DepsPath, $json, [Text.UTF8Encoding]::new($false))
