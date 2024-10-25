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

public class CreatePlaylistFeatureTests
{
    private readonly IClock _clock;
    private readonly IPlaylistRepository _playlistRepository;

    public CreatePlaylistFeatureTests()
    {
        _playlistRepository = Substitute.For<IPlaylistRepository>();
        _clock = Substitute.For<IClock>();
    }

    [Fact]
    public async Task CreatePlaylistHandler_ThrowsForbiddenException_IfUserIsNotAuthenticated()
    {
        var command = new CreatePlaylist.Command
        {
            Name = "Name",
            Public = true
        };

        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        authService.GetCurrentUserId().Returns((int?)null);

        mediator.Send(Arg.Any<CreatePlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreatePlaylistFeature.Handler(_playlistRepository, authService, _clock);
                return handler.Handle(callInfo.Arg<CreatePlaylist.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
    }

    [Fact]
    public async Task CreatePlaylistHandler_ReturnsCreatedPlaylistId()
    {
        var command = new CreatePlaylist.Command
        {
            Name = "Name",
            Public = true
        };

        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        authService.GetCurrentUserId().Returns(123);
        playlistRepository.Create(Arg.Any<Playlist>()).Returns(1);

        mediator.Send(Arg.Any<CreatePlaylist.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreatePlaylistFeature.Handler(playlistRepository, authService, _clock);
                return handler.Handle(callInfo.Arg<CreatePlaylist.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        result.Should().Be(1);
        await playlistRepository.Received().Create(Arg.Any<Playlist>());
    }
}