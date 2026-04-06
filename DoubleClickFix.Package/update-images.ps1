# update-images.ps1
# Regenerates all MSIX package Images from media/icon.png
# Run from anywhere; paths are relative to this script's location.

Add-Type -AssemblyName System.Drawing

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$repoRoot   = Split-Path -Parent $scriptDir
$iconSrc    = Join-Path $repoRoot "media\icon.png"
$imagesDir  = Join-Path $scriptDir "Images"

function Resize-Icon {
    param([string]$dest, [int]$w, [int]$h)
    $src = [System.Drawing.Bitmap]::new($iconSrc)
    $bmp = [System.Drawing.Bitmap]::new($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode     = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode   = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.DrawImage($src, 0, 0, $w, $h)
    $g.Dispose()
    $src.Dispose()
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "  $([System.IO.Path]::GetFileName($dest)) ${w}x${h}"
}

function Center-IconOnCanvas {
    param([string]$dest, [int]$canvasW, [int]$canvasH, [int]$iconSize)
    $src = [System.Drawing.Bitmap]::new($iconSrc)
    $bmp = [System.Drawing.Bitmap]::new($canvasW, $canvasH, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode     = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode   = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    $x = [int](($canvasW - $iconSize) / 2)
    $y = [int](($canvasH - $iconSize) / 2)
    $g.DrawImage($src, $x, $y, $iconSize, $iconSize)
    $g.Dispose()
    $src.Dispose()
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "  $([System.IO.Path]::GetFileName($dest)) ${canvasW}x${canvasH} (icon ${iconSize}px centered)"
}

Write-Host "Source: $iconSrc"
Write-Host "Output: $imagesDir"
Write-Host ""

# Square icons — simple resize
Resize-Icon (Join-Path $imagesDir "StoreLogo.png")                                          50  50
Resize-Icon (Join-Path $imagesDir "Square44x44Logo.scale-200.png")                         88  88
Resize-Icon (Join-Path $imagesDir "Square44x44Logo.targetsize-24_altform-unplated.png")    24  24
Resize-Icon (Join-Path $imagesDir "LockScreenLogo.scale-200.png")                          48  48
Resize-Icon (Join-Path $imagesDir "Square150x150Logo.scale-200.png")                      300 300

# Wide / splash — icon centered on transparent canvas
Center-IconOnCanvas (Join-Path $imagesDir "Wide310x150Logo.scale-200.png")  620 300 240
Center-IconOnCanvas (Join-Path $imagesDir "SplashScreen.scale-200.png")    1240 600 400

Write-Host ""
Write-Host "Done. Rebuild the MSIX in Visual Studio to pick up the new images."
