Add-Type -AssemblyName System.Drawing

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir
$sourcePng = Join-Path $scriptDir "plusui.png"

Write-Host "Source: $sourcePng"

# Load source image
$source = [System.Drawing.Image]::FromFile($sourcePng)

function Resize-Image($img, $width, $height, $outputPath) {
    $newImg = New-Object System.Drawing.Bitmap $width, $height
    $graphics = [System.Drawing.Graphics]::FromImage($newImg)
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $graphics.DrawImage($img, 0, 0, $width, $height)
    $newImg.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $newImg.Dispose()
    Write-Host "Created: $outputPath"
}

# iOS Icons
$iosBase = Join-Path $rootDir "samples\PlusUi.Demo.ios\Assets.xcassets\AppIcon.appiconset"
$iosSizes = @(20, 29, 40, 58, 60, 76, 80, 87, 120, 152, 167, 180, 1024)
foreach ($size in $iosSizes) {
    Resize-Image $source $size $size (Join-Path $iosBase "Icon$size.png")
}

# Android Icons (foreground only - background is solid color)
$androidDensities = @{
    "mdpi" = 48
    "hdpi" = 72
    "xhdpi" = 96
    "xxhdpi" = 144
    "xxxhdpi" = 192
}
foreach ($density in $androidDensities.Keys) {
    $size = $androidDensities[$density]
    $path = Join-Path $rootDir "samples\PlusUi.Demo.Android\Resources\mipmap-$density"
    Resize-Image $source $size $size (Join-Path $path "appicon.png")
    Resize-Image $source $size $size (Join-Path $path "appicon_foreground.png")
}

# Web Icons
$webBase = Join-Path $rootDir "samples\PlusUi.Demo.Web\wwwroot"
Resize-Image $source 32 32 (Join-Path $webBase "favicon.png")
Resize-Image $source 192 192 (Join-Path $webBase "icon-192.png")

# Template icon
Resize-Image $source 128 128 (Join-Path $rootDir "templates\plusui-app\.template.config\icon.png")

# ICO for DebugServer
$icoPath = Join-Path $rootDir "source\PlusUi.DebugServer\app.ico"
$bitmap = New-Object System.Drawing.Bitmap $source
$icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
$fs = New-Object System.IO.FileStream $icoPath, 'Create'
$icon.Save($fs)
$fs.Close()
$bitmap.Dispose()
Write-Host "Created: $icoPath"

$source.Dispose()
Write-Host "Done!"
