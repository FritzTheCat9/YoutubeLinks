# 03-fix-source-and-binary-incompat: Resolve compile-time incompatibilities and API breaking changes

Address Api.0001/Api.0002/Api.0003 findings from assessment. Replace or refactor code for APIs that changed, update source usage patterns, and apply small behavioral mitigations. Favor minimal, surgical changes that restore compilation and preserve behavior.

Affected items: Projects flagged with Api.* rules (see assessment.md for locations).

Done when: Solution builds with zero errors and no new warnings introduced by the upgrade work in modified projects.
