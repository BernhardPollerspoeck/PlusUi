Add-Type -AssemblyName System.Drawing
$png = [System.Drawing.Image]::FromFile('E:\Gewerbe\Pollerspöck\PlusUi\gfx\plusui.png')
$bitmap = New-Object System.Drawing.Bitmap $png
$icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
$fs = New-Object System.IO.FileStream 'E:\Gewerbe\Pollerspöck\PlusUi\source\PlusUi.DebugServer\app.ico', 'Create'
$icon.Save($fs)
$fs.Close()
$bitmap.Dispose()
$png.Dispose()
Write-Host 'ICO erstellt'
