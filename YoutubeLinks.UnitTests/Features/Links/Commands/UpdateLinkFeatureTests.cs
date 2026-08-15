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
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    [Fact]
    public async Task UpdateLinkHandler_ThrowsNotFoundException_IfPlaylistNotFound()
    {
        var command = new UpdateLink.Command { Id = 1, Url = "http://youtube.com?v=test" };
        _playlistRepository.FindPlaylistContainingLink(1).Returns((Playlist)null);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, default));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsForbiddenException_IfUserNotOwner()
    {
        var user = User.Create("u@u.com", "user", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("Test", false, user);
        playlist.AddLink("https://youtu.be/test", "title");

        _playlistRepository.FindPlaylistContainingLink(1).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(false);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(new() { Id = 1, Url = "x" }, default));
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsValidation_IfVideoIdInvalid()
    {
        var user = User.Create("u@u.com", "usr", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("p", false, user);
        playlist.AddLink("https://youtu.be/test", "title");

        _playlistRepository.FindPlaylistContainingLink(1).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() =>
            handler.Handle(new() { Id = 1, Url = "" }, default));
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsValidation_IfUrlExistsInOtherLinks()
    {
        var user = User.Create("u@u.com", "usr", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("p", false, user);
        playlist.AddLink("https://youtu.be/test", "title");

        _playlistRepository.FindPlaylistContainingLink(1).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);
        playlist.LinkUrlExistsInOtherLinksThan("https://youtu.be/test", 1).Returns(true);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() =>
            handler.Handle(new() { Id = 1, Url = "https://youtu.be/test" }, default));
    }

    [Fact]
    public async Task UpdateLinkHandler_FetchesTitle_WhenEmpty()
    {
        var user = User.Create("u@u.com", "usr", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("p", false, user);
        var link = playlist.AddLink("https://youtu.be/test", "");

        _playlistRepository.FindPlaylistContainingLink(link.Id).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);
        playlist.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>()).Returns(false);
        _youtubeService.GetVideoTitle(Arg.Any<string>()).Returns("New Title");

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);
        await handler.Handle(new() { Id = link.Id, Url = "https://youtu.be/test" }, default);

        Assert.Equal("New Title", link.Title);
        await _playlistRepository.Received().Update(playlist);
    }

    [Fact]
    public async Task UpdateLinkHandler_FetchesTitle_WhenUrlChanged()
    {
        var user = User.Create("u@u.com", "usr", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("p", false, user);
        var link = playlist.AddLink("https://youtu.be/old", "Title");

        _playlistRepository.FindPlaylistContainingLink(link.Id).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);
        playlist.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>()).Returns(false);
        _youtubeService.GetVideoTitle(Arg.Any<string>()).Returns("New Video Title");

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);

        await handler.Handle(new() { Id = link.Id, Url = "https://youtu.be/new" }, default);

        Assert.Equal("New Video Title", link.Title);
        await _playlistRepository.Received().Update(playlist);
    }

    [Fact]
    public async Task UpdateLinkHandler_DoesNotFetchTitle_WhenUnchanged()
    {
        var user = User.Create("u@u.com", "usr", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("p", false, user);
        var link = playlist.AddLink("https://youtu.be/test", "Existing Title");

        _playlistRepository.FindPlaylistContainingLink(link.Id).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);

        var handler = new UpdateLinkFeature.Handler(_playlistRepository, _authService, _youtubeService, _localizer);

        await handler.Handle(new() { Id = link.Id, Url = "https://youtu.be/test", Title = "Existing Title" }, default);

        await _youtubeService.DidNotReceive().GetVideoTitle(Arg.Any<string>());
        await _playlistRepository.Received().Update(playlist);
    }
}
