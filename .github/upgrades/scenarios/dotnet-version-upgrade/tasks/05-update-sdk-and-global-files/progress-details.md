# Progress Details - 05-update-sdk-and-global-files

## What I changed
- Added `global.json` to pin the SDK to the installed preview:
  - `11.0.100-preview.7.26381.103`
- Attempted to align Microsoft ASP.NET/Extensions packages to preview versions, but that caused package downgrade conflicts across projects. Reverted package version changes and left package versions at 10.0.11 where they were previously pinned.

## Validation
- Ran: `dotnet restore "YoutubeLinks.slnx"` and `dotnet build "YoutubeLinks.slnx"`
- Result: Restore and build succeeded with 1 non-blocking warning (NU1510 advising removal of redundant Microsoft.Extensions.Configuration.Abstractions package reference in YoutubeLinks.Shared).

## Notes
- Upgrading Microsoft.AspNetCore.* packages to preview versions introduced downgrade conflicts due to mixed package constraints across projects. A coordinated package upgrade across all projects (and possibly CI) is required to move Microsoft packages to preview safely.
- The global.json pin ensures local builds use the requested preview SDK. CI pipelines will need to be updated separately to install or use the preview SDK.

## Next steps
1. If you want to proceed with promoting Microsoft packages to preview, I can create a focused plan to upgrade them in a coordinated way across projects (update Directory.Packages.props + resolve transitive conflicts).
2. Otherwise continue with task 06 (run-and-fix-tests) — unit tests already passing.

## Modified files
- global.json

