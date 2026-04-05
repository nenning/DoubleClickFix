$releases = curl.exe -s -H "Accept: application/vnd.github+json" "https://api.github.com/repos/nenning/DoubleClickFix/releases" | ConvertFrom-Json

$releases | ForEach-Object {
    $version = $_.tag_name
    $_.assets | ForEach-Object {
        [PSCustomObject]@{
            Version   = $version
            Asset     = $_.name
            Downloads = $_.download_count
        }
    }
} | Format-Table -AutoSize