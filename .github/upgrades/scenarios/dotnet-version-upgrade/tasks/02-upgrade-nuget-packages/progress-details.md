# Progress Details - 02-upgrade-nuget-packages

## What I changed
- Updated central package versions in Directory.Packages.props:
  - `MediatR`: `12.4.1` -> `14.2.0`

## Validation
- Ran: `dotnet restore "YoutubeLinks.slnx"` and `dotnet build "YoutubeLinks.slnx"`
- Result: Restore and full solution build succeeded with 5 non-blocking warnings. Key warnings noted in per-project builds include Blazor analyzer warnings and an obsolete Testcontainers constructor in IntegrationTests.

## Issues encountered
- Some packages reported as "Not found at the sources" in earlier checks (Blazored packages) — these remain as-is; they may require adding additional feeds or manual review.
- Non-blocking warnings should be reviewed and addressed in subsequent tasks (03-fix-source-and-binary-incompat or package-specific updates).

## Next steps
- Continue scanning and updating other packages where newer stable versions are available. For packages with no compatible version or deprecations, document alternatives and open follow-up subtasks.

## Modified files
- Directory.Packages.props

