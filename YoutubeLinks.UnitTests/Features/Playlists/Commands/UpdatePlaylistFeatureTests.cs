using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands;

public class UpdatePlaylistFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IClock _clock = Substitute.For<IClock>();

    [Fact]
    public async Task UpdatePlaylistHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var command = new UpdatePlaylist.Command
        {
            Id = 1,
            Name = "Test",
            Public = true
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        mediator.Send(Arg.Any<UpdatePlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdatePlaylistFeature.Handler(playlistRepository, _authService, _clock);
                return handler.Handle(callInfo.Arg<UpdatePlaylist.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyNotFoundException>(action);
        await playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdatePlaylistHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUser()
    {
        var command = new UpdatePlaylist.Command
        {
            Id = 1,
            Name = "Test",
            Public = true
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        mediator.Send(Arg.Any<UpdatePlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdatePlaylistFeature.Handler(playlistRepository, authService, _clock);
                return handler.Handle(callInfo.Arg<UpdatePlaylist.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
        await playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdatePlaylistHandler_UpdatesPlaylist()
    {
        var command = new UpdatePlaylist.Command
        {
            Id = 1,
            Name = "Test",
            Public = true
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        mediator.Send(Arg.Any<UpdatePlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdatePlaylistFeature.Handler(playlistRepository, authService, _clock);
                return handler.Handle(callInfo.Arg<UpdatePlaylist.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        await playlistRepository.Received().Update(Arg.Any<Playlist>());
    }
}