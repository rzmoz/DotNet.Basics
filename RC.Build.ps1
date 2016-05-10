$parameters = Get-Content -Raw -Path .\RC.Params.json | ConvertFrom-Json

#init settings
$branch = git rev-parse --abbrev-ref HEAD
Write-Host "Current branch: $branch"
$commitHash = git rev-parse --verify HEAD
Write-Host "Commit hash: $commitHash"
$shortHash = git log --pretty=format:'%h' -n 1
Write-Host "Short hash: $shortHash"
$versionAssembly = $parameters."version.assembly"
Write-Host "Version Assembly: $versionAssembly"
$versionPrerelase= $parameters."version.prerelease"
Write-Host "Version Prerelease: $versionPrerelase"
$commitsSinceInit = git rev-list --first-parent --count HEAD
Write-Host "# of commits: $commitsSinceInit"
$semver10 = $versionAssembly
if(-Not [string]::IsNullOrWhiteSpace($versionPrerelase)) {
    $semver10 +="-$versionPrerelase"
}
Write-Host "SemVer 1.0: $semver10"
$semver20 = "$semver10+$commitsSinceInit.$commitHash"
Write-Host "SemVer 2.0: $semver20"

#patch assembly infos
$assemblyInformationalVersionPresenceInAllFiles = $true

Get-ChildItem $path -Filter "AssemblyInfo.cs" -recurse | Where-Object { $_.Attributes -ne "Directory"} |
    Update-AssemblyInfoVersions $versionAssembly $semver20

Function Update-AssemblyInfoVersions
{
  Param ([string]$Version,[string]$SemVer20)
    $NewVersion = ‘AssemblyVersion("‘ + $Version + ‘")’;
    $NewFileVersion = ‘AssemblyFileVersion("‘ + $Version + ‘")’;
    $NewInformationalVersion = ‘AssemblyInformationalVersion("‘ + $SemVer20 + ‘")’;
    
  foreach ($o in $input)
  {
    Write-output $o.FullName

    $TmpFile = $o.FullName + “.tmp”
    Copy-Item $o.FullName $TmpFile


     get-content $o.FullName |

        %{$_ -replace ‘AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)’, $NewVersion } |
        %{$_ -replace ‘AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)’, $NewFileVersion } |        
        %{$_ -replace ‘AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)’, $NewInformationalVersion }   > $o.FullName
  }
}

