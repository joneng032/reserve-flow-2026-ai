param(
    [string]$Project = "reserve flow ai 2026.csproj",
    [string]$Configuration = "Debug"
)

Write-Host "Running ListCompileItems msbuild target for project: $Project (Configuration=$Configuration)"
dotnet msbuild "$Project" -t:ListCompileItems -p:Configuration=$Configuration

$files = Get-ChildItem -Path ".\obj" -Recurse -Filter "compile_items_*.txt" -File -ErrorAction SilentlyContinue
if (-not $files -or $files.Count -eq 0) {
    Write-Error "No compile_items_*.txt files found under obj/. The target may have not run or wrote files to an unexpected location."
    exit 1
}

$empty = $files | Where-Object { $_.Length -eq 0 }
if ($empty) {
    Write-Error "Found empty compile_items files:`n$($empty | ForEach-Object { $_.FullName } | Out-String)"
    exit 1
}

Write-Host "OK: Found $($files.Count) compile_items files."
Write-Host "Files:`n$($files | ForEach-Object { $_.FullName } | Out-String)"
