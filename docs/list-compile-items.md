# ListCompileItems — MSBuild audit target

This document explains the `ListCompileItems` MSBuild target added to the MAUI project and how to run and validate it locally and in CI.

What it does

- `ListCompileItemsSingle` prints and writes the `@(Compile)` item list for a single `$(TargetFramework)` into `$(IntermediateOutputPath)`.

- `ListCompileItems` is a wrapper that either invokes the single-target for the current `$(TargetFramework)` or iterates `$(TargetFrameworks)` and invokes the per-TFM target for each framework.

Why we added it

- MAUI single-project projects include broad globs ("**/*.cs") and can accidentally pick up other projects' sources or generated `obj` files. This target produces per-TFM lists so we can audit included compile items and catch duplicate-type or accidental inclusion issues early in CI.

Run locally

1. From the repo root run (PowerShell):

```powershell
# simple invocation (will generate files under obj/<Configuration>/<tfm>/)
dotnet msbuild "reserve flow ai 2026.csproj" -t:ListCompileItems -p:Configuration=Debug

# or use the helper script which also validates presence: 
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\verify_list_compile_items.ps1
```

Expected outputs

- `obj\Debug\<tfm>\compile_items_<tfm>.txt` — per-TFM intermediate file

- `obj\Debug\compile_items_<tfm>.txt` — stable repo-root copy (for CI)

CI

- We run a lightweight workflow that builds the MAUI project and runs the `ListCompileItems` target and uploads per-TFM artifacts. See `.github/workflows/list-compile-and-check.yml` and `.github/workflows/verify-list-compile-items.yml`.

Troubleshooting

- If the workflow can't find `compile_items_*.txt` files, verify the `IntermediateOutputPath` and that the build target ran for each TFM. The helper script prints precise paths and fails with an error if files are missing or empty.

Notes

- The target writes both intermediate copies (under each TFM RIDs) and a stable repo-root `obj` copy to make CI discovery more robust across runner configurations.
