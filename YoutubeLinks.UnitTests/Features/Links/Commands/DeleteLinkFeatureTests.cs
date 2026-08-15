using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class DeleteLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task DeleteLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
    {
        var command = new DeleteLink.Command { Id = 1 };

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>())
            .Returns(Task.FromResult<Playlist>(null));

        var handler = new DeleteLinkFeature.Handler(
            _authService,
            _playlistRepository);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task DeleteLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUser()
    {
        var command = new DeleteLink.Command { Id = 1 };
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>())
            .Returns(Task.FromResult(playlist));
        _authService.IsLoggedInUser(Arg.Any<int>())
            .Returns(false);

        var handler = new DeleteLinkFeature.Handler(
            _authService,
            _playlistRepository);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task DeleteLinkHandler_DeletesValidLink()
    {
        var command = new DeleteLink.Command { Id = 1 };
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        var link = Link.Create("https://youtu.be/test", "Test Video", playlist);

        typeof(Link).GetProperty("Id")!.SetValue(link, 1);

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>())
            .Returns(Task.FromResult(playlist));

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var handler = new DeleteLinkFeature.Handler(_authService, _playlistRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.Empty(playlist.Links);  // Confirm link removed
        await _playlistRepository.Received().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task DeleteLinkHandler_ThrowsNotFound_WhenLinkDoesNotExist()
    {
        var command = new DeleteLink.Command { Id = 99 };

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>())
            .Returns(Task.FromResult<Playlist>(null));

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var handler = new DeleteLinkFeature.Handler(_authService, _playlistRepository);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }
}
