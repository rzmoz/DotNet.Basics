Function Update-AssemblyInfoVersions
{
  Param (
  [string]$Version,[
  string]$SemVer20)
  
  foreach ($o in $input)
  {
    Write-host $o.FullName -foregroundcolor green
    $TmpFile = $o.FullName + ".tmp"   
  
    #backup file for reverting later
    Copy-Item $o.FullName $TmpFile

    [regex]$patternAssemblyVersion = "(AssemblyVersion\("")(\d+\.\d+\.\d+\.\d+)(""\))"
    $replacePatternAssemblyVersion = "`${1}$($Version)`$3"
    [regex]$patternAssemblyFileVersion = "(AssemblyFileVersion\("")(\d+\.\d+\.\d+\.\d+)(""\))"
    $replacePatternAssemblyFileVersion = "`${1}$($Version)`$3"
    [regex]$patternAssemblyInformationalVersion = "(AssemblyInformationalVersion\("")(\d+\.\d+\.\d+\.\d+)(""\))"
    $replacePatternAssemblyInformationalVersion = "`${1}$($SemVer20)`$3"

     # run the regex replace        
     Get-Content -Path $o.FullName |
        % { $_ -replace $patternAssemblyVersion, $replacePatternAssemblyVersion } |
        % { $_ -replace $patternAssemblyFileVersion, $replacePatternAssemblyFileVersion } |
        % { $_ -replace $patternAssemblyInformationalVersion, $replacePatternAssemblyInformationalVersion }
     <#
    Get-Content -Path $o.FullName | % { $_ -replace $patternAssemblyVersion, $replacePatternAssemblyVersion }
    Get-Content -Path $o.FullName | % { $_ -replace $patternAssemblyFileVersion, $replacePatternAssemblyFileVersion } 
    Get-Content -Path $o.FullName | % { $_ -replace $patternAssemblyInformationalVersion, $replacePatternAssemblyInformationalVersion } 
    #>
  }
}

#init settings
$parameters = Get-Content -Raw -Path .\RC.Params.json | ConvertFrom-Json

$branch = git rev-parse --abbrev-ref HEAD
Write-Host "branch: $branch"  -foregroundcolor green
$commitHash = git rev-parse --verify HEAD
Write-Host "hash: $commitHash" -foregroundcolor green
$shortHash = git log --pretty=format:'%h' -n 1
Write-Host "hash.short: $shortHash" -foregroundcolor green
$versionAssembly = $parameters."version.assembly"
Write-Host "version.assembly: $versionAssembly" -foregroundcolor green
$versionPrerelase= $parameters."version.prerelease"
Write-Host "version.prerelease: $versionPrerelase" -foregroundcolor green
$commitsSinceInit = git rev-list --first-parent --count HEAD
Write-Host "# of commits: $commitsSinceInit" -foregroundcolor green
$semver10 = $versionAssembly
if(-Not [string]::IsNullOrWhiteSpace($versionPrerelase)) {
    $semver10 +="-$versionPrerelase"
}
Write-Host "semVer10: $semver10" -foregroundcolor green
$semver20 = "$semver10+$commitsSinceInit.$commitHash"
Write-Host "semVer20: $semver20" -foregroundcolor green

#patch assembly infos
$assemblyInformationalVersionPresenceInAllFiles = $true

Get-ChildItem .\ -Filter "AssemblyInfo.cs" -recurse | Where-Object { $_.Attributes -ne "Directory"} |
    Update-AssemblyInfoVersions $versionAssembly $semver20
