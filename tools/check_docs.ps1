param(
    [string]$Base = $(git rev-parse --show-toplevel 2>$null)
)

if (-not $Base) {
    Write-Error "Not a git repository or unable to find repo root."
    exit 2
}

Push-Location $Base

# Get changed files in the current branch vs main
$changed = git --no-pager diff --name-only origin/main...HEAD

if (-not $changed) {
    Write-Output "No changed files detected against origin/main."
    Pop-Location
    exit 0
}

$codeChanged = $changed -match '^(src/|Views/|Platforms/|Resources/|App.xaml|MauiProgram.cs)'
$docsChanged = $changed -match '^docs/'

if ($codeChanged -and -not $docsChanged) {
    Write-Output "Documentation not updated for code changes. The following files changed:`n$changed"
    Write-Output "Please update files under docs/ (CHANGELOG.md and relevant docs) or explain in your PR."
    Pop-Location
    exit 1
}

Write-Output "Docs OK or no relevant code changes detected."
Pop-Location
exit 0
