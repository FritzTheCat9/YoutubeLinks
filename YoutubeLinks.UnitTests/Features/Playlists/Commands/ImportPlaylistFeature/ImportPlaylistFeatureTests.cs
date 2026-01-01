using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using ApiFeature = YoutubeLinks.Api.Features.Playlists.Commands.ImportPlaylistFeature.ImportPlaylistFeature;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands.ImportPlaylistFeature;

public class ImportPlaylistFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    private readonly IStringLocalizer<ApiValidationMessage> _localizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task ImportPlaylistHandler_ThrowsForbiddenException_IfUserIsNotAuthenticated()
    {
        var command = new ImportPlaylist.Command
        {
            Name = "Name",
            Public = true,
            PlaylistFileType = PlaylistFileType.Json
        };

        _authService.GetCurrentUserId().Returns((int?)null);

        var handler = new ApiFeature.Handler(_playlistRepository, _userRepository, _authService, _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ImportPlaylistHandler_ReturnsImportedPlaylistId_IfImportFromJSONFileWorks()
    {
        var command = new ImportPlaylist.Command
        {
            Name = "Name",
            Public = true,
            PlaylistFileType = PlaylistFileType.Json,
            ExportedLinks =
            [
                new LinkJsonModel
                {
                    Title = "Rick Astley - Never Gonna Give You Up",
                    Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                    VideoId = "dQw4w9WgXcQ"
                }
            ]
        };

        _authService.GetCurrentUserId().Returns(123);
        _playlistRepository.Create(Arg.Any<Playlist>()).Returns(1);

        var handler = new ApiFeature.Handler(_playlistRepository, _userRepository, _authService, _youtubeService, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result);
        await _playlistRepository.Received().Create(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task ImportPlaylistHandler_ReturnsImportedPlaylistId_IfImportFromTXTFileWorks()
    {
        var command = new ImportPlaylist.Command
        {
            Name = "Name",
            Public = true,
            PlaylistFileType = PlaylistFileType.Txt,
            ExportedLinkUrls =
            [
                "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
            ]
        };

        _authService.GetCurrentUserId().Returns(123);
        _playlistRepository.Create(Arg.Any<Playlist>()).Returns(1);
        _youtubeService.GetVideoTitle(Arg.Any<string>()).Returns("Rick Astley - Never Gonna Give You Up");


        var handler = new ApiFeature.Handler(_playlistRepository, _userRepository, _authService, _youtubeService, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result);
        await _playlistRepository.Received().Create(Arg.Any<Playlist>());
    }
}