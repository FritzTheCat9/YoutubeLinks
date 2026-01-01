using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands;

public class ResetLinksDownloadedFlagFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task Handler_ThrowsNotFoundException_WhenPlaylistNotFound()
    {
        var command = new ResetLinksDownloadedFlag.Command { Id = 1, IsDownloaded = false };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new ResetLinksDownloadedFlagFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().SetLinksDownloadedFlag(Arg.Any<Playlist>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Handler_ThrowsForbiddenException_WhenUserIsNotOwner()
    {
        var command = new ResetLinksDownloadedFlag.Command { Id = 1, IsDownloaded = false };

        var playlist = PlaylistBuilder.Create()
            .WithName("TestPlaylist")
            .Public(false)
            .WithUser(UserBuilder.Create().WithEmail("testuser@gmail.com").Build())
            .Build();

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new ResetLinksDownloadedFlagFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().SetLinksDownloadedFlag(Arg.Any<Playlist>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task Handler_SetsPlaylistLinksDownloadedFlag_WhenUserIsOwner()
    {
        var command = new ResetLinksDownloadedFlag.Command { Id = 1, IsDownloaded = false };

        var playlist = PlaylistBuilder.Create()
            .WithName("TestPlaylist")
            .Public(false)
            .WithUser(UserBuilder.Create().WithEmail("testuser@gmail.com").Build())
            .Build();

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var handler = new ResetLinksDownloadedFlagFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        await _playlistRepository.Received().SetLinksDownloadedFlag(Arg.Any<Playlist>(), Arg.Any<bool>());
    }
}
