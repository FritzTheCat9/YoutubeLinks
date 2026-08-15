# .NET Version Upgrade

## Strategy
Upgrade all projects in the solution to net11.0 (Preview) using a project-by-project approach: update TargetFramework, then packages, then fix compilation and behavioral issues.

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net11.0 (Preview)

## Source Control
- **Source Branch**: fix/unit-tests-refactoring
- **Working Branch**: upgrade-dotnet-11
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
