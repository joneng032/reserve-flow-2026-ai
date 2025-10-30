# Versioning Guidance

We follow Semantic Versioning (SemVer): MAJOR.MINOR.PATCH

- MAJOR: incompatible API changes or major behavioral changes
- MINOR: new features that are backward-compatible
- PATCH: bug fixes and small improvements

Release process
1. Update `docs/CHANGELOG.md` with Unreleased changes.
2. Bump the version in the release commit and tag with `vMAJOR.MINOR.PATCH`.
3. Create a GitHub Release and move the changelog entries into the new version heading.

When to bump major
- Large breaking migrations (for example, database schema changes that require a migration step)

Keep release notes concise and actionable.
