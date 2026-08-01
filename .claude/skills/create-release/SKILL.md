---
name: create-release
description: Release checklist for DoubleClickFix — GitHub tag-triggered ZIP release and Microsoft Store MSIX packaging. Use when publishing a new release or asked how to cut a release.
---

**GitHub release (standalone ZIP):**
```bash
git tag -a v1.x.x.x
git push origin v1.x.x.x
```
This triggers the GitHub Action that builds and publishes the release. Add release notes on GitHub afterward.

**Microsoft Store package:** Use Visual Studio → Publish → Create App Packages (builds `.msixbundle` for x86/x64/arm64). Upload via Partner Portal.
