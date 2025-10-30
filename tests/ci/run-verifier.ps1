<#
runs the existing scripts/verify_list_compile_items.ps1 verifier and asserts that
`obj/<Configuration>/**/compile_items_*.txt` files exist and are non-empty.

Usage:
  pwsh -NoProfile -ExecutionPolicy Bypass -File .\tests\ci\run-verifier.ps1 -Project "reserve flow ai 2026.csproj" -Configuration Debug

This script is intended to be run on CI (Windows runner) as a small integration test.
#>

param(
    [string]$Project = "reserve flow ai 2026.csproj",
    [string]$Configuration = "Debug"
)

Set-StrictMode -Version Latest
Write-Host "[run-verifier] Project: $Project  Configuration: $Configuration"

try {
    # Locate the existing verifier script
    $verifierRelative = Join-Path $PSScriptRoot "..\..\scripts\verify_list_compile_items.ps1"
    $verifierPath = Resolve-Path -Path $verifierRelative -ErrorAction Stop
} catch {
    Write-Error "Could not find verifier script at $verifierRelative"
    exit 2
}

Write-Host "[run-verifier] Invoking verifier: $verifierPath"

# Run the verifier script in a fresh PowerShell process so its own -ErrorAction and exit codes are preserved.
# Prefer pwsh (PowerShell Core) when available, otherwise fallback to Windows PowerShell (powershell.exe).
$pwshCmd = $null
$cmd = Get-Command pwsh -ErrorAction SilentlyContinue
if ($cmd) { $pwshCmd = $cmd.Source } else {
    $cmd2 = Get-Command powershell.exe -ErrorAction SilentlyContinue
    if ($cmd2) { $pwshCmd = $cmd2.Source }
}

if (-not $pwshCmd) {
    Write-Error "No suitable PowerShell executable found (tried 'pwsh' and 'powershell.exe')."
    exit 2
}

Write-Host "[run-verifier] Using PowerShell: $pwshCmd"
& $pwshCmd -NoProfile -ExecutionPolicy Bypass -File $verifierPath -Project $Project -Configuration $Configuration
$verifierExit = $LASTEXITCODE
if ($null -eq $verifierExit) { $verifierExit = 0 }

if ($verifierExit -ne 0) {
    Write-Error "Verifier script exited with code $verifierExit"
    exit $verifierExit
}

# Now assert that compile_items files exist under obj\<Configuration>
$searchRoot = Join-Path (Get-Location) "obj\$Configuration"
Write-Host "[run-verifier] Searching for compile_items files under: $searchRoot"

if (-not (Test-Path $searchRoot)) {
    Write-Error "Search root does not exist: $searchRoot"
    exit 3
}

$files = Get-ChildItem -Path $searchRoot -Recurse -Filter 'compile_items_*.txt' -File -ErrorAction SilentlyContinue

if (-not $files -or ($files | Measure-Object).Count -eq 0) {
    Write-Error "No compile_items_*.txt files found under $searchRoot"
    # Provide a short diagnostic listing to help triage
    Get-ChildItem -Path $searchRoot -Recurse -File | Select-Object -First 50 | Format-Table -AutoSize
    exit 4
}

# Check for empty files
$zeroLength = $files | Where-Object { $_.Length -eq 0 }
if (($zeroLength | Measure-Object).Count -gt 0) {
    $emptyCount = ($zeroLength | Measure-Object).Count
    Write-Error "Found $emptyCount empty compile_items files. Listing them:"
    $zeroLength | ForEach-Object { Write-Error " - $($_.FullName) (0 bytes)" }
    exit 5
}

Write-Host "[run-verifier] OK: Found $((($files | Measure-Object).Count)) non-empty compile_items files."
Write-Host "[run-verifier] Files:"
$files | ForEach-Object { Write-Host " - $($_.FullName) : $($_.Length) bytes" }

exit 0
