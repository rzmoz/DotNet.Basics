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
  Write-Host "Assembly infos updated"
}
Function Undo-AssemblyInfoVersions
{
  foreach ($o in $input)
  {
    $TmpFile = $o.FullName + ".tmp"   
    Write-host "Reverting $TmpFile"
    Move-Item  $TmpFile $o.FullName -Force
  }
  Write-host "Assembly infos reverted"
}
