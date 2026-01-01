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
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class CreateLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task CreateLinkHandler_ThrowsValidationException_IfVideoIdIsNull()
    {
        var command = new CreateLink.Command
        {
            Url = "",
            PlaylistId = 1
        };
        var handler = new CreateLinkFeature.Handler(
            _playlistRepository,
            _authService,
            _youtubeService,
            _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, default));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=test123",
            PlaylistId = 1
        };

        _playlistRepository.Get(1)
            .Returns((Playlist)null);

        var handler = new CreateLinkFeature.Handler(
            _playlistRepository,
            _authService,
            _youtubeService,
            _localizer);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, default));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByUser()
    {
        var user = UserBuilder.Create().Build();
        var playlist = PlaylistBuilder.Create().WithUser(user).Build();
        var command = new CreateLink.Command { Url = "https://www.youtube.com/watch?v=test1", PlaylistId = 1 };

        _playlistRepository.Get(1).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(false);

        var handler = new CreateLinkFeature.Handler(
            _playlistRepository,
            _authService,
            _youtubeService,
            _localizer);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, default));
        await _playlistRepository.DidNotReceive().Update(Arg.Any<Playlist>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsValidationException_IfUrlAlreadyExists()
    {
        var user = UserBuilder.Create()
            .Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .WithLink("https://www.youtube.com/watch?v=test123", "test123", "Video1")
            .Build();
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=test123",
            PlaylistId = 1
        };

        _playlistRepository.Get(1).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);

        var handler = new CreateLinkFeature.Handler(
            _playlistRepository,
            _authService,
            _youtubeService,
            _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, default));
        await _playlistRepository.DidNotReceive().Update(playlist);
    }

    [Fact]
    public async Task CreateLinkHandler_ReturnsLinkId_WhenValid()
    {
        var user = UserBuilder.Create()
            .Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .Build();
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=test123",
            PlaylistId = 1
        };

        _playlistRepository.Get(1).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);
        _youtubeService.GetVideoTitle("test123").Returns("Sample Video");

        var handler = new CreateLinkFeature.Handler(
            _playlistRepository,
            _authService,
            _youtubeService,
            _localizer);

        var result = await handler.Handle(command, default);

        Assert.True(result > 0);
        await _playlistRepository.Received(1).Update(playlist);
    }
}
