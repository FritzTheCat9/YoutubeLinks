# 01-update-targetframeworks: Update project TargetFramework to net11.0

Update each project's TargetFramework to `net11.0` (or appropriate net11 target such as net11.0-windows if project requires Windows-only APIs). This is the minimal change to opt projects into the new runtime and SDK. Apply consistent TargetFramework values across projects; prefer single-targeting to net11.0 unless multi-targeting is required for shared libraries.

Affected items: All .csproj files in the solution.

Done when: All project files have TargetFramework (or TargetFrameworks) updated to include net11.0, and solution builds (or at least projects compile) after package updates and initial fixes.
