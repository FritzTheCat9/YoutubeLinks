using MediatR;
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

public class UpdateLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    private readonly IStringLocalizer<ApiValidationMessage> _localizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task UpdateLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsForbiddenException_IfUserIsNotLoggedIn()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsValidationException_IfVideoIdIsNull()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = string.Empty,
            Downloaded = false
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }


    [Fact]
    public async Task UpdateLinkHandler_ThrowsValidationException_IfLinkUrlExistsInOtherLinks()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var mockPlaylist = Substitute.For<Playlist>();
        mockPlaylist.LinkUrlExistsInOtherLinksThan(command.Url, command.Id).Returns(true);

        _playlistRepository.Get(Arg.Any<int>()).Returns(mockPlaylist);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdateLinkHandler_GetVideoTitle_IfTitleIsEmpty()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", string.Empty);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);

        const string newTitle = "Rick Astley - Never Gonna Give You Up";

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlist.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>())
            .Returns(false);
        _youtubeService.GetVideoTitle(Arg.Any<string>()).Returns(newTitle);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(newTitle, link.Title);
        Assert.Equal(Unit.Value, result);
        await _playlistRepository.Received().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdateLinkHandler_GetVideoTitle_IfUrlChanged()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=b7k0a5hYnSI",
            Downloaded = false
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up");

        const string newTitle = "Natasha Bedingfield - Unwritten";

        _playlistRepository.Get(Arg.Any<int>()).Returns(link);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _playlistRepository.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(false);
        _youtubeService.GetVideoTitle(Arg.Any<string>()).Returns(newTitle);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(newTitle, link.Title);
        Assert.Equal(Unit.Value, result);
        await _playlistRepository.Received().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdateLinkHandler_DontCallGetVideoTitle_IfTitleIsNotEmptyAndUrlDidNotChanged()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Title = "Rick Astley - Never Gonna Give You Up",
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };
        var link = new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            },
            Title = "Rick Astley - Never Gonna Give You Up",
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        };
        const string oldTitle = "Rick Astley - Never Gonna Give You Up";

        _playlistRepository.Get(Arg.Any<int>()).Returns(link);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _playlistRepository.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(false);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService,
            _youtubeService, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(oldTitle, link.Title);
        Assert.Equal(Unit.Value, result);
        await _youtubeService.DidNotReceive().GetVideoTitle(Arg.Any<string>());
        await _playlistRepository.Received().Update(Arg.Any<Playlist>());
    }
}