# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade all solution projects to net11.0 (Preview)
**Scope**: All projects in the solution (YoutubeLinks.Api, YoutubeLinks.Blazor, YoutubeLinks.Sdk, YoutubeLinks.Shared, YoutubeLinks.UnitTests, YoutubeLinks.IntegrationTests, YoutubeLinks.E2E, docker-compose project references). Moderate-sized solution with inter-project references; some packages will require updates or replacements.

## Tasks

### 01-update-targetframeworks: Update project TargetFramework to net11.0

Update each project's TargetFramework to `net11.0` (or appropriate net11 target such as net11.0-windows if project requires Windows-only APIs). This is the minimal change to opt projects into the new runtime and SDK. Apply consistent TargetFramework values across projects; prefer single-targeting to net11.0 unless multi-targeting is required for shared libraries.

Affected items: All .csproj files in the solution.

Done when: All project files have TargetFramework (or TargetFrameworks) updated to include net11.0, and solution builds (or at least projects compile) after package updates and initial fixes.

---

### 02-upgrade-nuget-packages: Upgrade incompatible or deprecated NuGet packages

Identify packages flagged by assessment (deprecated or lacking net11 support). Update Directory.Packages.props (if used) or individual project PackageReference versions to versions compatible with net11.0. When a package has no compatible version, document alternatives and create follow-up subtasks.

Affected items: Directory.Packages.props, projects that reference deprecated packages (see assessment.md).

Done when: All package references are upgraded to net11-compatible versions or documented as blockers with alternatives.

---

### 03-fix-source-and-binary-incompat: Resolve compile-time incompatibilities and API breaking changes

Address Api.0001/Api.0002/Api.0003 findings from assessment. Replace or refactor code for APIs that changed, update source usage patterns, and apply small behavioral mitigations. Favor minimal, surgical changes that restore compilation and preserve behavior.

Affected items: Projects flagged with Api.* rules (see assessment.md for locations).

Done when: Solution builds with zero errors and no new warnings introduced by the upgrade work in modified projects.

---

### 04-upgrade-blazor-wasm-and-hosting: Apply Blazor-specific compatibility changes

For Blazor WebAssembly and Blazor Server projects, update any Blazor-specific packages, update CSS/Static assets handling if changed, and verify the hosting project's TargetFramework. For WASM, ensure runtime/Mono/wasm build targets are compatible with the net11 SDK preview (update Microsoft.AspNetCore.Components.WebAssembly.* packages as needed).

Affected items: YoutubeLinks.Blazor project and any client/host projects.

Done when: Blazor apps run locally (dev serve) and build without errors; client-side artifacts are produced and served.

---

### 05-update-sdk-and-global-files: Update global.json, Directory.Packages.props and CI configs

Update global.json (if present) to pin the preview SDK version when required, update Directory.Packages.props for centralized package versions, and adjust CI/build pipelines to use the installed preview SDK. Document any CI changes required for preview SDK availability.

Affected items: global.json, Directory.Packages.props, CI pipeline definitions.

Done when: local build uses the preview SDK successfully and CI changes are documented in the repo (no CI execution changes applied automatically).

---

### 06-run-and-fix-tests: Run unit, integration, and E2E tests and resolve failures

Run test suites (UnitTests, IntegrationTests, E2E). Fix test failures that result from framework or package changes. Document flaky tests and any environmental adjustments needed for the preview runtime.

Affected items: YoutubeLinks.UnitTests, YoutubeLinks.IntegrationTests, YoutubeLinks.E2E.

Done when: Unit and integration tests pass locally, and E2E tests run without regression (or outstanding failures are documented with remediation steps).

---

### 07-final-validation-and-cleanup: Final build, warnings cleanup, and documentation

Perform a final solution build, clean up remaining warnings introduced by changes (fix or explain them), update README or docs noting the net11 preview usage, and prepare a summary progress report.

Done when: Solution builds without errors, warnings cleaned or documented, and an upgrade summary is added to the scenario artifacts.
