# GitHub Issue: Implement Separate Versioning for windows-on-device-development Branch

## Issue Summary
Implement branch-specific versioning strategy for the `windows-on-device-development` branch to enable independent development tracking while maintaining compatibility with main branch releases.

**Status**: Open
**Priority**: High
**Labels**: enhancement, versioning, branch-management, electron
**Assignee**: Development Team
**Created**: October 23, 2025

## Objectives
- [x] Update package.json versions to `3.0.0-electron-alpha`
- [x] Implement branch-specific version tagging scheme
- [x] Document version mapping in VERSION.md
- [ ] Establish workflow for branch-to-main version transitions
- [ ] Create automated versioning scripts for branch releases

## Versioning Scheme
```
Branch Version: reserve-flow-desktop-3.0.0-electron-alpha-windows-on-device-development
Main Release: reserve-flow-desktop-3.0.0-win
```

## Implementation Steps

### ✅ 1. Package Configuration (Completed)
- [x] Update main package.json to `3.0.0-electron-alpha`
- [x] Update api_v3.0.0/package.json to `3.0.0-electron-alpha`
- [x] Add version metadata for branch tracking

### ✅ 2. Tagging Strategy (In Progress)
- [x] Branch releases: `reserve-flow-desktop-{version}-windows-on-device-development-{increment}`
- [ ] Main releases: `reserve-flow-desktop-{version}-win`
- [x] Create sample branch tag: `reserve-flow-desktop-3.0.0-electron-alpha-windows-on-device-development-001`

### ✅ 3. Documentation Updates (Completed)
- [x] Update VERSION.md with branch-specific versioning section
- [x] Document merge workflow and version transitions
- [x] Create version mapping table for components

### 🔄 4. Automation Scripts (Pending)
- [ ] Create npm scripts for branch-specific versioning
- [ ] Implement automated tagging for releases
- [ ] Add version validation checks

## Success Criteria
- [x] Branch can release independently without affecting main branch
- [ ] Version conflicts prevented during merges
- [x] Clear traceability of branch-specific features
- [ ] Smooth transition path for production releases

## Related Files
- `package.json` - Main application version (updated to 3.0.0-electron-alpha)
- `api_v3.0.0/package.json` - API server version (updated to 3.0.0-electron-alpha)
- `docs/VERSION.md` - Version documentation (updated with branch versioning)
- Branch: `windows-on-device-development`

## Current Status
- **Package versions updated**: Both main and API packages now use 3.0.0-electron-alpha
- **Documentation completed**: VERSION.md includes comprehensive branch versioning strategy
- **Sample tag created**: Demonstrates the branch-specific tagging scheme
- **Next steps**: Implement automation scripts and establish merge workflows

## Comments
- **October 23, 2025**: Initial implementation completed. Package versions and documentation updated. Sample branch tag created to demonstrate versioning scheme.
- **Next**: Create automation scripts for version management and establish branch-to-main merge workflow.

## Resolution Plan
1. Create npm versioning scripts for branch-specific releases
2. Implement automated git tagging for branch releases
3. Establish merge workflow documentation
4. Test version transitions between branch and main
5. Close issue when all automation is implemented and tested