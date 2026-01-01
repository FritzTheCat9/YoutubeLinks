using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class CreateLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    private readonly IStringLocalizer<ApiValidationMessage> _localizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task CreateLinkHandler_ThrowsValidationException_IfVideoIdIsNull()
    {
        var command = new CreateLink.Command
        {
            Url = string.Empty,
            PlaylistId = 1
        };

        var handler = new CreateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new CreateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUser()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(false);

        var handler = new CreateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsValidationException_IfLinkUrlIsNotUniqueInPlaylist()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);

        var mockPlaylist = Substitute.For<Playlist>();
        mockPlaylist.LinkUrlExists(command.Url).Returns(true);

        _playlistRepository.Get(Arg.Any<int>()).Returns(mockPlaylist);

        var handler = new CreateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ReturnsLinkId_ForValidLink()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _youtubeService.GetVideoTitle(Arg.Any<string>()).Returns(Task.FromResult("Test Video Title"));

        var mockPlaylist = Substitute.For<Playlist>();
        mockPlaylist.LinkUrlExists(command.Url).Returns(false);

        _playlistRepository.Get(Arg.Any<int>()).Returns(mockPlaylist);

        var handler = new CreateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result);
        await _playlistRepository.Received(1).Update(playlist);
    }
}