# Reserve Flow (Native .NET MAUI)

The canonical project documentation lives in the `docs/` folder. See `docs/README.md` for project overview, implementation plans, and migration guidance.

This repository contains the MAUI implementation scaffold and developer docs. For quick access to docs and governance, open `docs/README.md`.

Quick start (developer)
-----------------------
1. Read `docs/README.md` for high-level guidance.
2. Restore and build (PowerShell):

```powershell
dotnet restore
dotnet build -f net8.0
```

Open the solution in Visual Studio to run on a selected platform.

CI
--
A basic GitHub Actions CI skeleton is included in `.github/workflows/ci.yml` to build and test on Windows and macOS runners.

Contributing
------------
See the `docs_from_maui` documents for high-level architecture and implementation guidance. Please open issues or pull requests against `main`.

License
-------
Specify license here.
