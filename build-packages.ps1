#Requires -Version 5.1
<#
    Builds distributable packages for StockApp:

        dist\StockApp-<version>-x64.msi    WiX installer (EULA + install dir UI)
        dist\StockApp-<version>-x64.msix   MSIX package (desktop bridge)

    Usage:
        .\build-packages.ps1
        .\build-packages.ps1 -Version 1.2.0.0
        .\build-packages.ps1 -SkipPublish     # reuse the existing .\publish folder
        .\build-packages.ps1 -SkipMsi
        .\build-packages.ps1 -SkipMsix
#>
param(
    [string]$Version = '1.0.0.0',
    [switch]$SkipPublish,
    [switch]$SkipMsi,
    [switch]$SkipMsix
)

$ErrorActionPreference = 'Stop'

$Root       = Split-Path -Parent $MyInvocation.MyCommand.Path
$Tools      = Join-Path $env:LOCALAPPDATA 'StockAppBuild\tools'
$WinSdk     = Join-Path $env:LOCALAPPDATA 'StockAppBuild\winsdk'
$PublishDir = Join-Path $Root 'publish'
$DistDir    = Join-Path $Root 'dist'
$StageDir   = Join-Path $Root 'build\msix'
$WiXVersion = '5.0.2'
$SdkVersion = '10.0.26100.1'
$SdkFolder  = '10.0.26100.0'

function Assert-Exit([string]$Step) {
    if ($LASTEXITCODE -ne 0) { throw "$Step failed with exit code $LASTEXITCODE" }
}

function Install-Wix {
    if (Test-Path (Join-Path $Tools 'wix.exe')) { return }
    Write-Host "==> Installing WiX $WiXVersion"
    New-Item -ItemType Directory -Force -Path $Tools | Out-Null
    dotnet tool install --tool-path $Tools wix --version $WiXVersion
    Assert-Exit 'dotnet tool install wix'

    & (Join-Path $Tools 'wix.exe') extension add -g "WixToolset.UI.wixext/$WiXVersion"
    Assert-Exit 'wix extension add'
}

function Install-WinSdkTools {
    if (Test-Path (Join-Path $WinSdk 'makeappx.exe')) { return }
    Write-Host "==> Downloading Windows SDK BuildTools $SdkVersion (makeappx/signtool)"
    $nupkg = Join-Path $env:TEMP "Microsoft.Windows.SDK.BuildTools.$SdkVersion.nupkg"
    $url = "https://api.nuget.org/v3-flatcontainer/microsoft.windows.sdk.buildtools/$SdkVersion/microsoft.windows.sdk.buildtools.$SdkVersion.nupkg"
    Invoke-WebRequest -Uri $url -OutFile $nupkg -UseBasicParsing

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($nupkg)
    $prefix = "bin/$SdkFolder/x64/"
    foreach ($entry in $zip.Entries) {
        if ($entry.FullName.StartsWith($prefix) -and -not $entry.FullName.EndsWith('/')) {
            $relative  = $entry.FullName.Substring($prefix.Length)
            $destination = Join-Path $WinSdk $relative
            $directory   = Split-Path $destination -Parent
            if (-not (Test-Path $directory)) { New-Item -ItemType Directory -Force -Path $directory | Out-Null }
            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destination, $true)
        }
    }
    $zip.Dispose()
    Remove-Item $nupkg -Force

    if (-not (Test-Path (Join-Path $WinSdk 'makeappx.exe'))) { throw 'makeappx.exe was not extracted' }
}

function Publish-App {
    Write-Host "==> dotnet publish (Release, win-x64)"
    if (Test-Path $PublishDir) { Remove-Item $PublishDir -Recurse -Force }
    dotnet publish (Join-Path $Root 'StockApp.csproj') -c Release -r win-x64 --self-contained false -o $PublishDir
    Assert-Exit 'dotnet publish'
}

function New-AppxAssets([string]$OutDir) {
    Add-Type -AssemblyName System.Drawing
    $icon = [System.Drawing.Image]::FromFile((Join-Path $Root 'images\home.png'))
    $background = [System.Drawing.Color]::FromArgb(255, 30, 30, 30)

    $assets = @(
        @{ Name = 'StoreLogo.png';    W = 50;   H = 50  },
        @{ Name = 'SmallTile.png';    W = 71;   H = 71  },
        @{ Name = 'MediumTile.png';   W = 310;  H = 310 },
        @{ Name = 'LargeTile.png';    W = 310;  H = 310 },
        @{ Name = 'WideTile.png';     W = 620;  H = 300 },
        @{ Name = 'SplashScreen.png'; W = 620;  H = 300 }
    )

    foreach ($asset in $assets) {
        $bitmap = New-Object System.Drawing.Bitmap($asset.W, $asset.H)
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.Clear($background)

        $maxSide  = [math]::Min($asset.W, $asset.H)
        $iconSide = [int]($maxSide * 0.6)
        $scale    = [math]::Min($iconSide / $icon.Width, $iconSide / $icon.Height)
        $iconW    = [int]($icon.Width * $scale)
        $iconH    = [int]($icon.Height * $scale)
        $graphics.DrawImage($icon, [int](($asset.W - $iconW) / 2), [int](($asset.H - $iconH) / 2), $iconW, $iconH)

        $bitmap.Save((Join-Path $OutDir $asset.Name), [System.Drawing.Imaging.ImageFormat]::Png)
        $graphics.Dispose()
        $bitmap.Dispose()
    }
    $icon.Dispose()
}

function Build-Msi {
    Write-Host "==> Building MSI"
    New-Item -ItemType Directory -Force -Path $DistDir | Out-Null
    $output = Join-Path $DistDir "StockApp-$Version-x64.msi"
    & (Join-Path $Tools 'wix.exe') build (Join-Path $Root 'Package.wxs') -ext WixToolset.UI.wixext -d "Version=$Version" -pdbtype none -o $output
    Assert-Exit 'wix build'
    Write-Host "    $output"
}

function Build-Msix {
    Write-Host "==> Building MSIX"
    New-Item -ItemType Directory -Force -Path $DistDir | Out-Null
    if (Test-Path $StageDir) { Remove-Item $StageDir -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $StageDir | Out-Null

    Copy-Item -Path (Join-Path $PublishDir '*') -Destination $StageDir -Recurse -Force
    Remove-Item -Path (Join-Path $StageDir '*.pdb') -Force -ErrorAction SilentlyContinue
    Copy-Item -Path (Join-Path $Root 'Packaging\AppxManifest.xml') -Destination (Join-Path $StageDir 'AppxManifest.xml') -Force

    $manifestPath = Join-Path $StageDir 'AppxManifest.xml'
    $manifestXml = Get-Content -Path $manifestPath -Raw
    $manifestXml = [regex]::Replace($manifestXml, '(<Identity[^>]*\bVersion=")[^"]+(")', ('${1}' + $Version + '${2}'))
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($manifestPath, $manifestXml, $utf8NoBom)

    $assetsDir = Join-Path $StageDir 'Assets'
    New-Item -ItemType Directory -Force -Path $assetsDir | Out-Null
    New-AppxAssets $assetsDir

    $output = Join-Path $DistDir "StockApp-$Version-x64.msix"
    & (Join-Path $WinSdk 'makeappx.exe') pack /d $StageDir /p $output /o
    Assert-Exit 'makeappx pack'
    Write-Host "    $output"
}

Install-Wix
Install-WinSdkTools
if (-not $SkipPublish) { Publish-App }
if (-not $SkipMsi)     { Build-Msi }
if (-not $SkipMsix)    { Build-Msix }

Write-Host '==> Done'
Get-ChildItem $DistDir -File | Where-Object { $_.Extension -in '.msi', '.msix' } | ForEach-Object {
    Write-Host ("    {0}  {1:N1} MB" -f $_.Name, ($_.Length / 1MB))
}
