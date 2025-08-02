param (
    [string]$Version
)

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
msbuild DoubleClickFix/DoubleClickFix.csproj /p:Configuration=Release /p:Platform=AnyCPU
if ($LASTEXITCODE -ne 0) {
    Write-Error "Standalone release build failed. Aborting release."
    exit 1
}

# 5. Create Git tag
Write-Host "Creating git tag v$Version"
git add Directory.Build.props
git commit -m "Bump version to $Version"
git tag -a "v$Version" -m "Version $Version"

Write-Host "Release process complete."
Write-Host "Next steps:"
Write-Host "1. Push the commit and tag to GitHub: git push --follow-tags"
Write-Host "   This will trigger the GitHub Action to create the release."
Write-Host "2. Publish the package project from Visual Studio."
Write-Host "   Upload the generated .msixbundle from DoubleClickFix.Package/AppPackages to the Microsoft Partner Center."