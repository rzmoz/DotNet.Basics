Import-Module .\AssemblyInfo.Cmdlets.psm1 -force

##*********** Init ***********##
$parameters = Get-Content -Raw -Path .\RC.Params.json | ConvertFrom-Json

$artifactsDir = ".\Artifacts"
Write-Host "aritfacts dir: $artifactsDir" -ForegroundColor Cyan
$testResultsPath = "$artifactsDir\testresults.xml"
Write-Host "test results path: $testResultsPath" -ForegroundColor Cyan
$branch = git rev-parse --abbrev-ref HEAD
Write-Host "branch: $branch" -ForegroundColor Cyan
$commitHash = git rev-parse --verify HEAD
Write-Host "hash: $commitHash" -ForegroundColor Cyan
$shortHash = git log --pretty=format:'%h' -n 1
Write-Host "hash.short: $shortHash" -ForegroundColor Cyan
$versionAssembly = $parameters."version.assembly"
Write-Host "version.assembly: $versionAssembly" -ForegroundColor Cyan
$versionPrerelase= $parameters."version.prerelease"
Write-Host "version.prerelease: $versionPrerelase" -ForegroundColor Cyan
$commitsSinceInit = git rev-list --first-parent --count HEAD
Write-Host "# of commits: $commitsSinceInit" -ForegroundColor Cyan
$semver10 = $versionAssembly
if(-Not [string]::IsNullOrWhiteSpace($versionPrerelase)) {
    $semver10 +="-$versionPrerelase"
}
Write-Host "semVer10: $semver10" -ForegroundColor Cyan
$semver20 = "$semver10+$commitsSinceInit.$commitHash"
Write-Host "semVer20: $semver20" -ForegroundColor Cyan
$slnPath = $parameters."sln.path"
Write-Host "sln.path: $slnPath" -ForegroundColor Cyan
$msbuildConfiguration = $parameters."msbuild.configuration"
Write-Host "msbuild.configuration: $msbuildConfiguration" -ForegroundColor Cyan
$testAssembliesFilter = $parameters."test.assemblies"
Write-Host "test.assemblies filter: $testAssembliesFilter" -ForegroundColor Cyan
$nugetTargets = $parameters."nuget.targets"
foreach($nugetTarget in $nugetTargets.path) {
    Write-Host "nuget.target: $nugetTarget" -ForegroundColor Cyan
}

##*********** Build ***********##

<#

#clean repo for release - this will fail if everything is not committed
#https://git-scm.com/docs/git-clean
Write-Host "Cleaning repo for relase build"
git clean -d -x -f | Write-Host -ForegroundColor DarkGray

#patch assembly infos
$assemblyInfos = Get-ChildItem .\ -Filter "AssemblyInfo.cs" -recurse | Where-Object { $_.Attributes -ne "Directory"} 
$assemblyInfos | Update-AssemblyInfoVersions $versionAssembly $semver20

#restore nugets
#https://docs.nuget.org/consume/command-line-reference
Write-Host "Restoring nuget packages"
& ".\nuget.exe" restore $slnPath | Write-Host -ForegroundColor DarkGray

#build sln
Write-Host "Building $slnPath"
& "C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\MSBuild.exe" $slnPath /t:rebuild /p:Configuration=$msbuildConfiguration /verbosity:minimal | Write-Host -ForegroundColor DarkGray

#revert assembly info
$assemblyInfos | Undo-AssemblyInfoVersions

#>

#clean artifacts dir if exists
if(Test-Path $artifactsDir) { Remove-Item "$artifactsDir\*" -Force | Write-Host -ForegroundColor DarkGray }
#create aritfacts dir
New-Item $artifactsDir -ItemType Directory -Force | Write-Host -ForegroundColor DarkGray

#run unit tests
$testAssemblies = Get-ChildItem -Path .\ -Filter "$testAssembliesFilter" -Recurse | Where-Object { $_.FullName -like "*`\bin`\$msbuildConfiguration`\$testAssembliesFilter" -and $_.Attributes -ne "Directory" }
Write-Host $testAssemblies.FullName
#https://github.com/nunit/docs/wiki/Console-Command-Line
& "C:\Users\rar\OneDrive\Dev\nunit\nunit3-console.exe" $testAssemblies.FullName --framework:net-4.5 --result:$testResultsPath | Write-Host -ForegroundColor DarkGray

<#

#create nugets and place in artifacts dir
foreach($nugetTarget in $nugetTargets.path) {
#https://docs.nuget.org/consume/command-line-reference
    Write-Host "Packing $nugetTarget"
    & ".\nuget.exe" pack $nugetTarget -Properties "Configuration=$msbuildConfiguration;Platform=AnyCPU" -version $semver10 -OutputDirectory $artifactsDir  | Write-Host -ForegroundColor DarkGray
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
            Write-Host $_.FullName
            & ".\nuget.exe" push $_.FullName -ApiKey $apiKey -Source "https://api.nuget.org/v3/index.json" -NonInteractive | Write-Host -ForegroundColor DarkGray
        }
} else {
    Write-Host "Push to nuget dismissed" -ForegroundColor yellow
}
#>