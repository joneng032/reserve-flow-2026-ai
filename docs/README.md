# Reserve Flow Documentation Hub

This `docs/` folder is the canonical location for the project's user and developer-facing documentation.

What belongs here
- Master implementation plans (architecture, milestones)
- Migration maps and mapping artifacts
- CONTRIBUTING, CHANGELOG, and VERSIONING guidance
- Design tokens and style guidance (when ready)

Quick links
- `MASTER_IMPLEMENTATION_PLAN_MAUI.md` — The high-level plan and sprints
- `MIGRATION_MAP.md` — File-by-file mapping from React -> MAUI
- `CHANGELOG.md` — Project changelog (keep up to date for releases)
- `VERSIONING_GUIDANCE.md` — Versioning policy and release process

Where to update docs
- Update files in `docs/` for any architecture or behavioral change.
- Pull requests that modify code in `src/`, `Views/`, `Platforms/`, or other implementation folders should include updates to `docs/` when public behavior, APIs, or flows change.

Maintainers
- The `docs/` folder is owned by the Platform Architect and Lead Mobile Engineer. Use GitHub issues tagged `docs` to request changes or clarifications.

If you need to copy or consolidate existing files from `docs_from_maui/`, see `tools/sync_docs.ps1` (optional helper).
