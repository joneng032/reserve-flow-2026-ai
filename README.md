# Reserve Flow (Native .NET MAUI)

Native rewrite of the Reserve Flow app — MAUI-first implementation with feature parity to the previous React/Capacitor version.

Quick links
- Repository: https://github.com/joneng032/reserve-flow-2026-ai

Overview
--------
This repository contains a .NET MAUI implementation plan and the application scaffolding for Reserve Flow — a Smart Audio Notes and AI-assisted inspection tool with BLE/IoT integrations and study framework support.

Prerequisites
- .NET SDK (8.0+ or 9.0 depending on target)
- Visual Studio 2022/2023 with MAUI workloads (Windows) or Visual Studio for Mac (macOS)
- Android SDK and emulators (for Android builds)
- Xcode and macOS runner (for iOS builds)

Quick start
------------
1. Restore and build:

```powershell
cd "F:\github\reserve flow ai 2026"
dotnet restore
dotnet build -f net8.0
```

2. Run (platform-specific): open the solution in Visual Studio and run on the desired platform.

CI
--
A basic GitHub Actions CI skeleton is included in `.github/workflows/ci.yml` to build and test on Windows and macOS runners.

Contributing
------------
See the `docs_from_maui` documents for high-level architecture and implementation guidance. Please open issues or pull requests against `main`.

License
-------
Specify license here.
