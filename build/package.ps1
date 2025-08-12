param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

Write-Host "Packaging Lidarr.Recommendations plugin version $Version"

# Create artifacts directory
$artifactsDir = "artifacts"
if (Test-Path $artifactsDir) {
    Remove-Item $artifactsDir -Recurse -Force
}
New-Item -ItemType Directory -Path $artifactsDir | Out-Null

# Build the project
Write-Host "Building project..."
dotnet build src/Lidarr.Recommendations/Lidarr.Recommendations.csproj -c Release --no-restore

# Create package directory
$packageDir = Join-Path $artifactsDir "Lidarr.Recommendations"
New-Item -ItemType Directory -Path $packageDir | Out-Null

# Copy built files
$sourceDir = "src/Lidarr.Recommendations/bin/Release/net8.0"
Write-Host "Copying files from $sourceDir to $packageDir"

# Copy DLL and dependencies
Copy-Item "$sourceDir/Lidarr.Recommendations.dll" $packageDir
Copy-Item "$sourceDir/Lidarr.Recommendations.pdb" $packageDir -ErrorAction SilentlyContinue

# Copy plugin.json and update version
$pluginJsonPath = "src/Lidarr.Recommendations/plugin.json"
$pluginJson = Get-Content $pluginJsonPath | ConvertFrom-Json
$pluginJson.version = $Version
$pluginJson | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $packageDir "plugin.json")

# Copy UI files if they exist
$uiSource = "$sourceDir/UI"
if (Test-Path $uiSource) {
    Write-Host "Copying UI files..."
    Copy-Item $uiSource $packageDir -Recurse
}

# Copy Resources if they exist
$resourcesSource = "$sourceDir/Resources"
if (Test-Path $resourcesSource) {
    Write-Host "Copying Resources..."
    Copy-Item $resourcesSource $packageDir -Recurse
}

# Create ZIP package
$zipPath = Join-Path $artifactsDir "Lidarr.Recommendations-v$Version.zip"
Write-Host "Creating package: $zipPath"

if (Get-Command Compress-Archive -ErrorAction SilentlyContinue) {
    Compress-Archive -Path "$packageDir/*" -DestinationPath $zipPath
} else {
    # Fallback for older PowerShell versions
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($packageDir, $zipPath)
}

Write-Host "Package created successfully: $zipPath"
Write-Host "Package contents:"
Get-ChildItem $packageDir -Recurse | ForEach-Object { Write-Host "  $($_.FullName.Substring($packageDir.Length + 1))" }