Function Update-AssemblyInfoVersions
{
  Param (
  [string]$Version,[
  string]$SemVer20)
  
  foreach ($o in $input)
  {
    $fullName=$o.FullName
    Write-host "Updating $fullName"
    $TmpFile = $o.FullName + ".tmp"   
    Write-host "Backup: $TmpFile"  -ForegroundColor DarkGray

    #backup file for reverting later
    Copy-Item $o.FullName $TmpFile

    [regex]$patternAssemblyVersion = "(AssemblyVersion\("")(\d+\.\d+\.\d+\.\d+)(""\))"
    $replacePatternAssemblyVersion = "`${1}$($Version)`$3"
    [regex]$patternAssemblyFileVersion = "(AssemblyFileVersion\("")(\d+\.\d+\.\d+\.\d+)(""\))"
    $replacePatternAssemblyFileVersion = "`${1}$($Version)`$3"
    [regex]$patternAssemblyInformationalVersion = "(AssemblyInformationalVersion\("")(\d+\.\d+\.\d+\.\d+)(""\))"
    $replacePatternAssemblyInformationalVersion = "`${1}$($SemVer20)`$3"

     # run the regex replace        
     $updated = Get-Content -Path $o.FullName |
        % { $_ -replace $patternAssemblyVersion, $replacePatternAssemblyVersion } |
        % { $_ -replace $patternAssemblyFileVersion, $replacePatternAssemblyFileVersion } |
        % { $_ -replace $patternAssemblyInformationalVersion, $replacePatternAssemblyInformationalVersion }
     Set-Content $o.FullName -Value $updated -Force
  }
}
Function Revert-AssemblyInfoVersions
{
  foreach ($o in $input)
  {
    $TmpFile = $o.FullName + ".tmp"   
    Write-host "Reverting $TmpFile"
    Move-Item  $TmpFile $o.FullName -Force
  }
  Write-host "Assembly infos reverted"
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

$assemblyInfos = Get-ChildItem .\ -Filter "AssemblyInfo.cs" -recurse | Where-Object { $_.Attributes -ne "Directory"} 

$assemblyInfos | Update-AssemblyInfoVersions $versionAssembly $semver20
Write-Host "Assembly infos updated"
$assemblyInfos | Revert-AssemblyInfoVersions
