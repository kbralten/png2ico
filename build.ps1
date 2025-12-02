$ErrorActionPreference = "Stop"

$projectPath = "Png2Ico/Png2Ico.csproj"
$outputDir = "dist"
$installerDir = "Installer"

Write-Host "Building Png2Ico..."

# Clean up previous build
if (Test-Path $outputDir) {
    Write-Host "Cleaning existing dist folder..."
    Remove-Item -Path $outputDir -Recurse -Force
}

# Build and Publish
# -c Release: Optimize for production
# -r win-x64: Target Windows 64-bit
# --self-contained false: Relies on installed .NET runtime (smaller file size)
# -p:PublishSingleFile=true: Bundles everything into one .exe
# -o $outputDir: Output directory
dotnet publish $projectPath -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o $outputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "EXE Build complete successfully!"
    Write-Host "Executable located at: $(Resolve-Path $outputDir\Png2Ico.exe)"
} else {
    Write-Host "EXE Build failed."
    exit 1
}

# Check for WiX
if (Get-Command "wix" -ErrorAction SilentlyContinue) {
    Write-Host "Building MSI Installer..."
    
    # Ensure Installer directory exists (it should)
    if (-not (Test-Path $installerDir)) {
        Write-Host "Installer directory not found."
        exit 1
    }

    # Run WiX build
    # We change location to Installer directory for the command to ensure relative paths work
    Push-Location $installerDir
    try {
        wix build Package.wxs -o Png2Ico.msi
        if ($LASTEXITCODE -eq 0) {
            Write-Host "MSI Build complete successfully!"
            Write-Host "Installer located at: $(Resolve-Path Png2Ico.msi)"
        } else {
            Write-Host "MSI Build failed."
        }
    }
    finally {
        Pop-Location
    }
} else {
    Write-Host "WiX toolset not found. Skipping MSI build."
    Write-Host "To install WiX: dotnet tool install --global wix"
}
