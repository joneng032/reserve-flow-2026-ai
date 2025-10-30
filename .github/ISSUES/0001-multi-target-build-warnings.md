---
title: Multi-target MAUI build: duplicate-type (CS0436) and platform/RID warnings
labels: build, ci, maintenance
---

Summary
-------

The multi-target single-project MAUI solution currently emits a set of build warnings that should be tracked and remediated to stabilize CI and reduce noise. Key findings:

- Duplicate-type warnings (CS0436) were observed when a ProjectReference (ReserveFlow.Core) and MAUI single-project broad globs both included the same source files. A targeted csproj exclusion was added to the MAUI project to remove `src/ReserveFlow.Core/**` from MAUI's implicit globs and the duplicate-type failure is resolved.
- Platform-compatibility CA1416 warnings appear throughout where platform-specific APIs (Windows/Android/iOS/MacCatalyst) are used; these are expected but should be audited and platform-guarded where appropriate.
- NETSDK1206 warnings related to RID-specific native assets for SQLite (alpine-x64 notice).
- XA0141 warnings regarding Android page-size for packaged sqlite native libraries.

Impact
------

- CS0436 duplicates block a clean multi-target build and make test and CI diagnostics noisy.
- CA1416 warnings indicate platform-unsafe API usage on cross-targets and may cause runtime failures if not guarded.
- RID/native warnings may impact runtime on certain platforms or produce missing native asset failures in some CI images.

Proposed remediation plan (phased)
---------------------------------


1) (Done) Keep ReserveFlow.Core as a ProjectReference and explicitly remove its source folder from the MAUI project's implicit globs.

2) Add CI checks to fail the build on new CS0436/CS0579 duplicates. This prevents regressions where a new folder or glob brings sources back into MAUI.

3) Audit CA1416 warnings and add targeted platform guards (for example, `#if WINDOWS` or `OperatingSystem.IsWindows()` where appropriate) for code that must be platform-specific. For UI surface code, prefer runtime guards only when necessary; for services, prefer per-platform implementations under `Platforms/{Tfm}`.

4) Address RID/native warnings:
   - Pin or configure SQLitePCLRaw/native packages; consider adding explicit `RuntimeIdentifiers` for CI images that need native assets.
   - Follow guidance in the .NET RID usage docs and package vendor notes for Android page-size XA0141 (see: [RID usage guidance](https://aka.ms/dotnet/rid-usage)).

5) Add the `ListCompileItems` MSBuild target (in MAUI csproj) so maintainers can audit what the single-project globs include. Use it in CI for periodic audits.

6) Create small follow-up tasks:
   - Add a CI job that runs `dotnet msbuild -t:ListCompileItems` and uploads `obj/compile_items_*.txt` as a build artifact for quick diagnosis.
   - Consider adding a `Directory.Build.props` with conservative defaults that opt projects out of SDK-generated assembly attributes where appropriate.

Notes and attachments
---------------------

- Binary logs and previous diagnostics are available in workspace root (e.g., build_after_core_exclude.binlog).
- The MAUI csproj has been updated with the Core exclusion and the ListCompileItems target.

Next steps
----------

- Confirm CI is updated with the CS0436 gate and a ListCompileItems artifact upload.
- Prioritize RID/native remediation if CI test runners exhibit native-asset resolution failures.

RID / native remediation options (concrete)
-----------------------------------------

If CI workers report missing native assets or the NETSDK1206 / XA0141 warnings are a concern, here are pragmatic next steps you can adopt (pick/adjust per your CI matrix):

- Short term: pin and unify the SQLite native packaging versions. Add or update package references to `SQLitePCLRaw.bundle_e_sqlite3` (this bundle wires the appropriate e_sqlite3 native binaries) in projects that need it. Example in the MAUI project or ReserveFlow.Core:

   ```xml
   <PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.0.4" />
   ```

- Medium term: declare the `RuntimeIdentifiers` that your CI expects to build/publish for. That makes RID-specific assets available during restore and helps the SDK avoid NETSDK1206 notices. Example (MAUI csproj, adjust to target list used in CI):

   ```xml
   <PropertyGroup>
      <RuntimeIdentifiers>win10-x64;iossimulator-x64;maccatalyst-x64;android-x64;android-arm64</RuntimeIdentifiers>
   </PropertyGroup>
   ```

- If Android XA0141 remains, check for updated `SQLitePCLRaw.lib.e_sqlite3.android` packages that include 16KB page-size variants or contact package maintainers. Consider pinning an updated version when available.

- Long term: add a CI step that runs a short publish for each platform (or at least the RIDs of concern) to ensure native assets resolve and surface any runtime asset gaps before release.

If you want, I can open a small PR that adds a `SQLitePCLRaw.bundle_e_sqlite3` PackageReference to the MAUI project (or the Core project), and/or add a commented `RuntimeIdentifiers` block in the MAUI csproj for your CI team to enable if they choose.

If you'd like, I can open follow-up PR(s) to add the CI gate and artifact reporting.
