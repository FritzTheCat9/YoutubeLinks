using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using ApiFeature = YoutubeLinks.Api.Features.Playlists.Commands.ExportPlaylistFeature.ExportPlaylistFeature;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands.ExportPlaylistFeature;

public class ExportPlaylistFeatureTests
{
    private readonly IAuthService _authService;

    public ExportPlaylistFeatureTests()
    {
        _authService = Substitute.For<IAuthService>();
    }

    [Fact]
    public async Task ExportPlaylistHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Json
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        mediator.Send(Arg.Any<ExportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ApiFeature.Handler(playlistRepository, _authService);
                return handler.Handle(callInfo.Arg<ExportPlaylist.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyNotFoundException>(action);
    }

    [Fact]
    public async Task ExportPlaylistHandler_ThrowsForbiddenException_IfPlaylistIsNotPublicAndNotOwnedByLoggedInUser()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Json
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1,
            Public = false
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        mediator.Send(Arg.Any<ExportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ApiFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<ExportPlaylist.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
    }

    [Fact]
    public async Task ExportPlaylistHandler_ReturnsJSONPlaylistFile_IfPlaylistIsPublic()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Json
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1,
            Public = true,
            Name = "Name"
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        mediator.Send(Arg.Any<ExportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ApiFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<ExportPlaylist.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<PlaylistFile>();
        result.ContentType.Should().Be("application/json");
        result.FileName.Should().Be("Name.json");
        result.PlaylistFileType.Should().Be(PlaylistFileType.Json);
    }

    [Fact]
    public async Task ExportPlaylistHandler_ReturnsTXTPlaylistFile_IfUserIsLoggedIn()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Txt
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1,
            Public = false,
            Name = "Name"
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        mediator.Send(Arg.Any<ExportPlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ApiFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<ExportPlaylist.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<PlaylistFile>();
        result.ContentType.Should().Be("text/plain");
        result.FileName.Should().Be("Name.txt");
        result.PlaylistFileType.Should().Be(PlaylistFileType.Txt);
    }
}