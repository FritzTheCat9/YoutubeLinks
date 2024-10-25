using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands.ExportPlaylistFeature;

public class ExportPlaylistTests
{
    [Theory]
    [InlineData((PlaylistFileType)2)]
    [InlineData((PlaylistFileType)3)]
    [InlineData((PlaylistFileType)4)]
    [InlineData((PlaylistFileType)5)]
    public void ExportPlaylistValidator_PlaylistFileType_ShouldBeInEnum(PlaylistFileType playlistFileType)
    {
        var message = $"PlaylistFileType has a range of values which does not include: {playlistFileType}.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.PlaylistFileTypeIsInEnum), message);

        var validator = new ExportPlaylist.Validator(localizer);

        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = playlistFileType
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PlaylistFileType)
            .WithErrorMessage(message);
    }
}