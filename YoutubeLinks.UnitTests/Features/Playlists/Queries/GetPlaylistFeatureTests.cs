using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.UnitTests.Features.Playlists.Queries;

public class GetPlaylistFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    [Fact]
    public async Task GetPlaylistHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var query = new GetPlaylist.Query
        {
            Id = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        mediator.Send(Arg.Any<GetPlaylist.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetPlaylistFeature.Handler(playlistRepository, _authService);
                return handler.Handle(callInfo.Arg<GetPlaylist.Query>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(query, CancellationToken.None);

        await Assert.ThrowsAsync<MyNotFoundException>(action);
    }

    [Fact]
    public async Task GetPlaylistHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUserAndPlaylistIsNotPublic()
    {
        var query = new GetPlaylist.Query
        {
            Id = 1
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

        mediator.Send(Arg.Any<GetPlaylist.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetPlaylistFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<GetPlaylist.Query>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(query, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
    }

    [Fact]
    public async Task GetPlaylistandler_ReturnsPlaylistDto_IfPlaylistIsPublic()
    {
        var query = new GetPlaylist.Query
        {
            Id = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1,
            Public = true
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        mediator.Send(Arg.Any<GetPlaylist.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetPlaylistFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<GetPlaylist.Query>(), CancellationToken.None);
            });

        var result = await mediator.Send(query, CancellationToken.None);

        result.Should().BeOfType<PlaylistDto>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPlaylistHandler_ReturnsPlaylistDto_IfPlaylistIsOwnedByLoggedInUser()
    {
        var query = new GetPlaylist.Query
        {
            Id = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1,
            Public = false
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        mediator.Send(Arg.Any<GetPlaylist.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetPlaylistFeature.Handler(playlistRepository, authService);
                return handler.Handle(callInfo.Arg<GetPlaylist.Query>(), CancellationToken.None);
            });

        var result = await mediator.Send(query, CancellationToken.None);

        result.Should().BeOfType<PlaylistDto>();
        result.Should().NotBeNull();
    }
}