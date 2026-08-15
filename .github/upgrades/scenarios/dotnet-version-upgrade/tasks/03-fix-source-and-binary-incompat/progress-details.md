# Progress Details - 03-fix-source-and-binary-incompat

## What I changed
- Made YouTube video-id extraction more robust in `YoutubeLinks.Api/Data/ValueObjects/YoutubeUrl.cs` to support `v=` query, `youtu.be/` short links, and `embed/` URLs.
- Allowed creating `Link` with a null `Title` when an empty title is provided by deferring title validation until a title is set (changed `YoutubeLinks.Api/Data/Entities/Link.cs`).

## Validation
- Ran: `dotnet test "YoutubeLinks.UnitTests\YoutubeLinks.UnitTests.csproj"`
- Result: Tests executed; currently **9 tests failing**, 242 passing. Failures are from link/title-related tests and a small NSubstitute argument-spec issue surfacing in a few handler tests.

## Issues encountered
- Some unit tests still fail (validation/NSubstitute interactions) and need further triage. The failures are reproducible locally and appear in UpdateLinkFeature tests.

## Next steps
1. Triage remaining failing tests to determine precise root causes (handler logic, test setup assumptions, or additional incompatibilities).
2. Apply minimal fixes and run the tests iteratively until all pass.
3. After unit tests pass, run integration and E2E tests and fix any runtime issues.

## Modified files
- YoutubeLinks.Api/Data/ValueObjects/YoutubeUrl.cs
- YoutubeLinks.Api/Data/Entities/Link.cs


