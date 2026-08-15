# 05-update-sdk-and-global-files: Update global.json, Directory.Packages.props and CI configs

Update global.json (if present) to pin the preview SDK version when required, update Directory.Packages.props for centralized package versions, and adjust CI/build pipelines to use the installed preview SDK. Document any CI changes required for preview SDK availability.

Affected items: global.json, Directory.Packages.props, CI pipeline definitions.

Done when: local build uses the preview SDK successfully and CI changes are documented in the repo (no CI execution changes applied automatically).
