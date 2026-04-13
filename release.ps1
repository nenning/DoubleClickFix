param (
    [string]$Version
)

# Ensure we're on the master branch
$branch = git rev-parse --abbrev-ref HEAD
if ($branch -ne "master") {
    Write-Error "Must be on master branch to release. Current branch: $branch"
    exit 1
}

# Check for upstream changes before doing anything
Write-Host "Checking for upstream changes..."
git fetch origin 2>&1 | Out-Null
$upstreamChanges = git log HEAD..origin/master --oneline
if ($upstreamChanges) {
    Write-Warning "Upstream changes detected on origin/master. Pull before releasing:"
    Write-Host $upstreamChanges
    exit 1
}

# Locate msbuild via vswhere, fall back to dotnet
$msbuild = $null
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (Test-Path $vswhere) {
    $vsPath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
    if ($vsPath) {
        $candidate = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"
        if (Test-Path $candidate) {
            $msbuild = $candidate
        }
    }
}
if (-not $msbuild) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Error "Neither msbuild nor dotnet could be found. Install Visual Studio or the .NET SDK and try again."
        exit 1
    }
    Write-Host "msbuild not found via vswhere, will use 'dotnet build'."
}

# If no version is provided, read the current version and increment it
if (-not $Version) {
    Write-Host "No version argument provided. Auto-incrementing version..."
    $currentVersionLine = Get-Content Directory.Build.props | Select-String -Pattern '<Version>(.*)</Version>'
    $currentVersion = $currentVersionLine.Matches.Groups[1].Value

    if (-not $currentVersion) {
        Write-Error "Could not find version in Directory.Build.props. Please specify a version manually using -Version."
        exit 1
    }

    $versionParts = $currentVersion.Split('.')
    if ($versionParts.Length -ne 4) {
        Write-Error "Invalid version format found: $currentVersion. Expected format: Major.Minor.Build.Revision"
        exit 1
    }

    $buildNumber = [int]$versionParts[2]
    $buildNumber++
    $Version = "$($versionParts[0]).$($versionParts[1]).$buildNumber.$($versionParts[3])"
    Write-Host "Current version: $currentVersion -> New version: $Version"
}

# Update Directory.Build.props
(Get-Content Directory.Build.props) -replace '<Version>.*</Version>', "<Version>$Version</Version>" | Set-Content Directory.Build.props

Write-Host "Updated version to $Version in Directory.Build.props"

# Build the standalone release
Write-Host "Building standalone release for AnyCPU..."
if ($msbuild) {
    & $msbuild DoubleClickFix/DoubleClickFix.csproj /p:Configuration=Release /p:Platform=AnyCPU
} else {
    dotnet build DoubleClickFix/DoubleClickFix.csproj --configuration Release
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Standalone release build failed. Aborting release."
    exit 1
}

# 5. Create Git tag
Write-Host "Creating git tag v$Version"
git add Directory.Build.props
git commit Directory.Build.props -m "Bump version to $Version"
git tag -a "v$Version" -m "Version $Version"

# 6. Push the version bump commit and tag
Write-Host "Pushing to origin..."
git push origin master:master
git push origin "v$Version"

Write-Host "Release process complete. The GitHub Action will create the release."
Write-Host "Next steps:"
Write-Host "1. Add release notes on GitHub."
Write-Host "2. Publish the package project from Visual Studio."
Write-Host "   Upload the generated .msixbundle from DoubleClickFix.Package/AppPackages to the Microsoft Partner Center."