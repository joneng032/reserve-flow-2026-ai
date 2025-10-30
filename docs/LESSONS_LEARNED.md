# Lessons learned — MAUI CI audit and multi-target compile

Date: 2025-10-30

This document captures issues we encountered while adding a CI audit that lists per-target compile items for the MAUI single-project app, the fixes we applied, and short recommendations to make future development smoother. The goal is to treat this project as a learning exercise and to capture fixes and follow-ups as GitHub issues.

## Summary

We added a custom MSBuild target `ListCompileItems` to the MAUI project, a GitHub Actions workflow to build and run that target for CI, and a small audit that fails the build on duplicate-type warnings (CS0436/CS0579). The CI run required several fixes to command quoting, project targeting, MSBuild target ordering, and artifact upload globs.

## Issues encountered and solutions

- Problem: GitHub CLI PR creation failed locally due to authentication (HTTP 401).
  - Solution: Manually created the PR on github.com and continued iterating on CI via commits to the feature branch.

- Problem: PowerShell on Windows runners split the `-warnaserror` argument on `;` which broke the `dotnet build` invocation in the workflow.
  - Solution: Quote the `-warnaserror:CS0436;CS0579` argument so PowerShell doesn't split it. Example: `"-warnaserror:CS0436;CS0579"`.

- Problem: CI attempted to run an MSBuild target that only exists in the MAUI project when invoked at the solution level; target wasn't found for non-MAUI projects.
  - Solution: Target the MAUI csproj directly in CI: `dotnet build "reserve flow ai 2026.csproj" -t:ListCompileItems`.

- Problem: The custom `ListCompileItems` target ran too early in the build lifecycle in some invocations and produced empty artifact files.
  - Solution: Add `DependsOnTargets="Build"` to the `ListCompileItems` target so it runs after normal compilation populates the Compile items and write the files into `obj/<config>/<tfm>/*`.

- Problem: Uploaded artifacts were empty or missing because the workflow's upload path pattern did not match all runtime-identifier subdirectories (.e.g `net9.0-ios/iossimulator-x64`).
  - Solution: Use a broader glob when uploading: `obj/**/compile_items_*.txt` and ensure the files are written to `obj/...` (CI runner working directory) before upload.

## What worked well

- The `ListCompileItems` MSBuild target gives us an audit trail for each config/TFM/RID combination. The files are small and easy to inspect locally or via CI artifacts.
- Treating CS0436/CS0579 as errors in CI prevented duplicate-type regressions early.
- Iterative CI debugging (fixing quoting, scoping to the project, and making artifact globs resilient) was straightforward and short.

## Remaining quality items (summary)

1. Platform compatibility warnings (CA1416): many call sites use Windows-only APIs in cross-platform code paths (e.g., `Platforms/Windows/WindowsSpeechService.cs`, `WindowsAudioService.cs`, `AppShell.xaml.cs`, some XAML pages). These are warnings but should be audited to ensure safe runtime behavior.
2. XAML compiled binding warnings (XC0022): several XAML pages can benefit from adding `x:DataType` for improved performance and better compile-time checking (e.g., `Views/DashboardPage.xaml`, `Views/ProjectDetailPage.xaml`).
3. CI artifact naming/collection improvements: ensure we upload one artifact per CI run with clear hierarchy (Debug/net9.0-*/rid/compile_items_*.txt) or a small zip per TFM for easier consumption.
4. Document and test the `ListCompileItems` target: add a short doc and a CI check that ensures the target writes files and that CI artifacts exist and are not empty.

## Recommended follow-ups (tracked as issues)

- Audit and mitigate CA1416 warnings (Windows-specific API uses).
- Add `x:DataType` to views with XC0022 warnings to enable compiled bindings and improve runtime performance.
- Harden CI artifact upload (use consistent paths, create per-TFM artifacts, and add validation to fail if artifacts are missing or empty).
- Add docs and automated tests for `ListCompileItems` target to ensure future changes don't break CI auditing.

## Notes for future development

- When adding new platform-specific implementations, prefer isolating them behind interfaces injected via DI. This keeps the shared code free of platform APIs and reduces CA1416 noise.
- Prefer compiler-time checks (x:DataType, analyzers) to catch problems early.
- Keep CI steps small and well-scoped so failures give actionable output (e.g., upload per-TFM artifacts, run analyzers separately so their output is visible in dedicated job steps).

---

Artifactual references:

- PR: chore: add Compile-items audit target, CI workflow draft, pin sqlite native bundle (active PR used during this audit)
- Files touched during the audit: `.github/workflows/list-compile-and-check.yml`, `reserve flow ai 2026.csproj` (added/updated target)

If you want, I can open the GitHub issues now for the follow-ups listed above and include links back to this document and the PR.
