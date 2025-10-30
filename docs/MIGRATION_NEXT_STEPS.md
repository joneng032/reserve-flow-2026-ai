# Migration — next steps (React -> MAUI)

This file is a living checklist to guide the ongoing migration of UI and app flows from the React codebase into this MAUI app.

Goals
- Safely port UI and business logic incrementally.
- Keep CI green and have small, reviewable PRs.
- Preserve high-priority functionality first (recording/camera/AI features), then polish UI.

Immediate checklist (next 2–4 sprints)
1. Inventory
   - List React pages/components and map them to MAUI Views/ViewModels.
   - Identify platform-specific features (camera, audio) and map to `Platforms/*` implementations.
2. Core flows
   - Port authentication and data model wiring.
   - Port navigation flows (AppShell -> Shell navigation routes).
3. High-priority screens
   - Dashboard, ProjectDetail, Recording, Camera capture pages.
4. Native dependencies
   - Verify SQLite native bundles and other native libs (we pinned sqlite in this branch).
5. Tests & CI
   - Add small integration tests for ported flows; ensure `ListCompileItems` verifier remains green.

Implementation guidance
- Port one page at a time: create a MAUI View + ViewModel pair, wire to navigation and test locally.
- Reuse existing business logic in `src/ReserveFlow.Core` where possible.
- Keep platform-specific code behind DI interfaces and implement in `Platforms/<OS>` folders.

Commit & PR strategy
- Keep PRs small (< 300 LOC ideally). Demonstrate the UI and functionality in the PR description.
- Run the CI verifier job on each PR — it will catch multi-target compile anomalies.

Immediate actions you can ask me to do now
- Create a component inventory by scanning the React repo (if you give me access or files).
- Create one sample migrated page (Dashboard) scaffolded with View + ViewModel and navigation registration.
- Create GitHub issues for CA1416/XC0022 follow-ups and link this doc.

Notes
- We're already running the `ListCompileItems` verifier in CI and have a small `tests/ci/run-verifier.ps1` wrapper. Keep these in place while migrating so CI warns about multi-target compile changes early.
