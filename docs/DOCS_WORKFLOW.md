# Docs Workflow and Automation

This document describes how we keep `docs/` up to date and the automated checks that help enforce it.

Rules
- `docs/` is the canonical source for project documentation and must contain:
  - `README.md` (project overview and quick start)
  - `MASTER_IMPLEMENTATION_PLAN_MAUI.md` (architecture & sprints)
  - `MIGRATION_MAP.md` (detailed mapping)
  - `CHANGELOG.md` and `VERSIONING_GUIDANCE.md`

Developer expectations
- When you change behavior, API surface, or add a feature, update `docs/` to explain the change.
- Update `CHANGELOG.md` under `Unreleased` for user-facing changes.
- For architecture changes, update the relevant master docs and mention the PR in `docs/`.

Automation
- A GitHub Actions workflow (`.github/workflows/docs-guard.yml`) runs on PRs. It checks whether the PR modifies implementation files (for example `src/`, `Views/`, `Platforms/`) and whether corresponding documentation under `docs/` was updated. If a PR changes implementation but not docs, the workflow fails and posts a comment linking to the docs template.
- A lightweight local helper `tools/check_docs.ps1` is included so authors can run the same check before opening a PR.

Escalation & exceptions
- Small cosmetic changes (typos) may not require changelog updates. If you intentionally omit a docs change, add a short explanation in the PR description.

Ownership
- Docs updates should be reviewed by at least one reviewer listed in CODEOWNERS (or a designated docs reviewer).
