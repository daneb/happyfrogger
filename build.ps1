# HappyFrogger build script (Windows)
# Requires: tools/tailwindcss.exe (see README for download instructions)
param(
    [switch]$Serve,
    [switch]$Drafts,
    [int]$Port = 4000
)

$tailwind  = ".\tools\tailwindcss.exe"
$outputCss = "..\blog\output.css"

if (Test-Path $tailwind) {
    Write-Host "Building Tailwind CSS..."
    & $tailwind -i input.css -o $outputCss --minify
} else {
    Write-Warning "Tailwind CLI not found at $tailwind"
    Write-Host "Download from https://github.com/tailwindlabs/tailwindcss/releases"
    Write-Host "Place as tools\tailwindcss.exe"
    Write-Host "Skipping CSS build — using existing output.css if present."
}

Write-Host "Generating site..."
$passArgs = @()
if ($Serve)            { $passArgs += "--serve" }
if ($Drafts)           { $passArgs += "--drafts" }
if ($Port -ne 4000)    { $passArgs += "--port"; $passArgs += $Port }

dotnet run --project HappyFrog.csproj -- @passArgs
