using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Abstractions;
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
    private readonly IClock _clock = Substitute.For<IClock>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
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

        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        authService.GetCurrentUserId().Returns((int?)null);

        mediator.Send(Arg.Any<ImportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ApiFeature.Handler(_playlistRepository, authService, _youtubeService, _clock,
                    _localizer);
                return handler.Handle(callInfo.Arg<ImportPlaylist.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
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

        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        authService.GetCurrentUserId().Returns(123);
        playlistRepository.Create(Arg.Any<Playlist>()).Returns(1);

        mediator.Send(Arg.Any<ImportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ApiFeature.Handler(playlistRepository, authService, _youtubeService, _clock,
                    _localizer);
                return handler.Handle(callInfo.Arg<ImportPlaylist.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);
        result.Should().Be(1);
        await playlistRepository.Received().Create(Arg.Any<Playlist>());
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

        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var youtubeService = Substitute.For<IYoutubeService>();
        var mediator = Substitute.For<IMediator>();

        authService.GetCurrentUserId().Returns(123);
        playlistRepository.Create(Arg.Any<Playlist>()).Returns(1);
        youtubeService.GetVideoTitle(Arg.Any<string>()).Returns("Rick Astley - Never Gonna Give You Up");

        mediator.Send(Arg.Any<ImportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler =
                    new ApiFeature.Handler(playlistRepository, authService, youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<ImportPlaylist.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);
        result.Should().Be(1);
        await playlistRepository.Received().Create(Arg.Any<Playlist>());
    }
}