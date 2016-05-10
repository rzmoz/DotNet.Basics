Import-Module .\AssemblyInfo.Cmdlets.psm1 -verbose -force

##*********** Init ***********##
$parameters = Get-Content -Raw -Path .\RC.Params.json | ConvertFrom-Json

Write-Host "nuget API key: $nugetApiKey" -foregroundcolor green
$artifactsDir = ".\Artifacts"
Write-Host "aritfacts dir: $artifactsDir" -foregroundcolor green
$branch = git rev-parse --abbrev-ref HEAD
Write-Host "branch: $branch" -foregroundcolor green
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
$slnPath = $parameters."sln.path"
Write-Host "sln.path: $slnPath" -foregroundcolor green
$msbuildConfiguration = $parameters."msbuild.configuration"
Write-Host "msbuild.configuration: $msbuildConfiguration" -foregroundcolor green
$testAssemblies = $parameters."test.assemblies"
Write-Host "test.assemblies: $testAssemblies" -foregroundcolor green
$nugetTargets = $parameters."nuget.targets"
foreach($nugetTarget in $nugetTargets.path) {
    Write-Host "nuget.target: $nugetTarget" -foregroundcolor green
}

foreach ($o in $input) {
    Write-Host $o -foregroundcolor blue
}

##*********** Build ***********##

#clean repo for release - this will fail if everything is not committed
#https://git-scm.com/docs/git-clean
git clean -d -x -f

#patch assembly infos
$assemblyInfos = Get-ChildItem .\ -Filter "AssemblyInfo.cs" -recurse | Where-Object { $_.Attributes -ne "Directory"} 
$assemblyInfos | Update-AssemblyInfoVersions $versionAssembly $semver20

#restore nugets
#https://docs.nuget.org/consume/command-line-reference
& ".\nuget.exe" restore $slnPath

#build sln
& "C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\MSBuild.exe" $slnPath /t:rebuild /p:Configuration=$msbuildConfiguration

#revert assembly info
$assemblyInfos | Undo-AssemblyInfoVersions

#run unit tests
#https://github.com/nunit/dev/wiki/Command-Line-Options
& ".\nunit3-console.exe" "$testAssemblies"

<#

#create aritfacts dir
New-Item $artifactsDir -ItemType Directory

#create nugets and place in artifacts dir
foreach($nugetTarget in $nugetTargets.path) {
#https://docs.nuget.org/consume/command-line-reference
    & ".\nuget.exe" pack $nugetTarget -Properties "Configuration=$msbuildConfiguration;Platform=AnyCPU" -version $semver10 -OutputDirectory $artifactsDir
}

#pust to nuget.org
$message  = 'Pust to nuget.org'
$question = 'Do you want to publish nuget packages to nuget.org?'

$choices = New-Object Collections.ObjectModel.Collection[Management.Automation.Host.ChoiceDescription]
$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Yes'))
$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&No'))

$decision = $Host.UI.PromptForChoice($message, $question, $choices, 1)
if ($decision -eq 0) {
    $apiKey = Read-Host "Please enter nuget API key"
    #https://docs.nuget.org/consume/command-line-reference
    Get-ChildItem $artifactsDir -Filter "*.nupkg" | % { 
            Write-Host $_.FullName -ForegroundColor yellow
            & ".\nuget.exe" push $_.FullName -ApiKey $apiKey -Source "https://api.nuget.org/v3/index.json" -NonInteractive              
        }
} else {
    Write-Host "Push to nuget dismissed" -ForegroundColor yellow
}
#>