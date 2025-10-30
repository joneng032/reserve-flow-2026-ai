# Branch-Specific Versioning Workflow

## Overview

This document outlines the workflow for managing branch-specific versioning in the Reserve Flow project, particularly for the `windows-on-device-development` branch. This ensures independent development tracking while maintaining compatibility with main branch releases.

## Branch Versioning Strategy

### Branch-Specific Versions
- **Branch**: `windows-on-device-development`
- **Version Format**: `3.0.0-electron-alpha-windows-on-device-development-{increment}`
- **Tag Format**: `reserve-flow-desktop-3.0.0-electron-alpha-windows-on-device-development-{increment}`

### Main Branch Versions
- **Branch**: `main`
- **Version Format**: `3.0.0` (clean semver)
- **Tag Format**: `reserve-flow-desktop-3.0.0-win`

## Development Workflow

### 1. Branch Development

When working on the `windows-on-device-development` branch:

```bash
# Switch to branch
git checkout windows-on-device-development
git pull origin windows-on-device-development

# Make changes and commit
# ... development work ...

# Bump version for branch development
npm run version:bump-branch

# Push changes and tags
git push origin windows-on-device-development
git push origin --tags
```

### 2. Branch Release Preparation

Before creating a release from the branch:

```bash
# Validate current version
npm run version:validate

# Prepare release (patch, minor, or major)
npm run version:prepare patch  # or 'minor' or 'major'

# Create and push tag
npm run version:tag
git push origin --tags
```

### 3. Merge to Main Branch

When ready to merge branch changes to main:

```bash
# Create pull request from windows-on-device-development to main
# After PR approval and merge:

git checkout main
git pull origin main

# Transition to main branch versioning
npm run version:merge-main

# Push the version change
git push origin main
git push origin --tags
```

## Automated Scripts

### Available NPM Scripts

```bash
# Prepare a new release version
npm run version:prepare [patch|minor|major]

# Create git tag for current version
npm run version:tag

# Validate current version against branch
npm run version:validate

# Bump version and create tag for branch development
npm run version:bump-branch

# Handle version transition when merging to main
npm run version:merge-main
```

### Manual Script Usage

```bash
# Prepare release
node scripts/branch-versioning.js prepare-release patch

# Create tag
node scripts/branch-versioning.js tag-release

# Validate version
node scripts/branch-versioning.js validate-version

# Bump branch version
node scripts/branch-versioning.js bump-branch

# Merge to main transition
node scripts/branch-versioning.js merge-to-main
```

## Version Validation

The versioning system includes automatic validation:

- **Branch versions** must follow the expected pattern for each branch
- **Main branch** versions must be clean semver (no prerelease identifiers)
- **Feature branches** allow standard semver with optional prerelease tags

Run validation anytime:
```bash
npm run version:validate
```

## Tag Naming Convention

### Branch Development Tags
```
reserve-flow-desktop-3.0.0-electron-alpha-windows-on-device-development-001
reserve-flow-desktop-3.0.0-electron-alpha-windows-on-device-development-002
...
```

### Main Branch Release Tags
```
reserve-flow-desktop-3.0.0-win
reserve-flow-desktop-4.0.0-win
...
```

### Feature Branch Tags (if needed)
```
reserve-flow-capacitor-4.6.0-feature-name
```

## Conflict Prevention

### Version Conflicts
- Branch-specific versions include branch identifiers to prevent conflicts
- Main branch versions are always clean semver
- Automated validation prevents invalid version formats

### Merge Conflicts
- Version files are updated automatically during merge transitions
- Manual review required for version conflicts
- CI/CD pipelines validate version consistency

## CI/CD Integration

### Branch Builds
- Branch-specific builds use branch version numbers
- Artifacts are tagged with branch-specific identifiers
- Separate deployment channels for branch releases

### Main Branch Builds
- Main branch builds use clean version numbers
- Production deployments from main branch only
- Version validation gates prevent invalid releases

## Troubleshooting

### Common Issues

#### Version Validation Fails
```bash
# Check current branch and version
git branch --show-current
npm version

# Validate version
npm run version:validate

# If invalid, check branch configuration in scripts/branch-versioning.js
```

#### Tag Creation Fails
```bash
# Check if tag already exists
git tag | grep "reserve-flow"

# Delete existing tag if needed
git tag -d <tag-name>
git push origin :refs/tags/<tag-name>

# Try again
npm run version:tag
```

#### Merge Conflicts in package.json
```bash
# During merge, if package.json has conflicts:
git checkout --theirs package.json  # Accept incoming changes
# OR
git checkout --ours package.json    # Keep current changes

# Then run version transition
npm run version:merge-main
```

### Getting Help

1. Check this documentation first
2. Review the VERSIONING_ISSUE.md for context
3. Run validation scripts to identify issues
4. Check git logs for recent version changes

## Examples

### Complete Branch Development Cycle

```bash
# Start development
git checkout windows-on-device-development
git pull origin windows-on-device-development

# Make changes
echo "feat: add new feature" > commit-msg.txt
git add .
git commit -F commit-msg.txt

# Bump version
npm run version:bump-branch
git push origin windows-on-device-development --tags

# Create release
npm run version:prepare minor
npm run version:tag
git push origin windows-on-device-development --tags

# Merge to main
# (via GitHub PR)
git checkout main
git pull origin main
npm run version:merge-main
git push origin main --tags
```

### Feature Branch Development

```bash
# Create feature branch
git checkout -b feature/new-feature
git pull origin main

# Development work
# ... make changes ...

# Version bump (optional for feature branches)
npm version patch
git add package.json
git commit -m "chore: bump version for feature release"

# Merge back to main
git checkout main
git merge feature/new-feature
```

## Maintenance

### Regular Tasks
- Review version tags quarterly
- Clean up old development tags annually
- Update branch configurations as needed
- Validate version consistency across branches

### Configuration Updates
- Update `BRANCH_CONFIGS` in `scripts/branch-versioning.js` for new branches
- Modify validation rules as versioning strategy evolves
- Update this documentation when workflow changes

---

*Branch-Specific Versioning Workflow - Reserve Flow Platform*
*Last Updated: October 26, 2025*