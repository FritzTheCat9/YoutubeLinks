# 02-upgrade-nuget-packages: Upgrade incompatible or deprecated NuGet packages

Identify packages flagged by assessment (deprecated or lacking net11 support). Update Directory.Packages.props (if used) or individual project PackageReference versions to versions compatible with net11.0. When a package has no compatible version, document alternatives and create follow-up subtasks.

Affected items: Directory.Packages.props, projects that reference deprecated packages (see assessment.md).

Done when: All package references are upgraded to net11-compatible versions or documented as blockers with alternatives.
