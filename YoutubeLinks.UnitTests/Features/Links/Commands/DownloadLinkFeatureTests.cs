using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class DownloadLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task DownloadLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
    {
        var command = new DownloadLink.Command
        {
            Id = 1,
            YoutubeFileType = YoutubeFileType.Mp3
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new DownloadLinkFeature.Handler(_authService, _playlistRepository, _youtubeService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task
        DownloadLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUserOrPlaylistIsNotPublic()
    {
        var command = new DownloadLink.Command
        {
            Id = 1,
            YoutubeFileType = YoutubeFileType.Mp3
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new DownloadLinkFeature.Handler(_authService, _playlistRepository, _youtubeService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DownloadLinkHandler_UserPlaylist_ReturnsYoutubeFile()
    {
        var command = new DownloadLink.Command
        {
            Id = 1,
            YoutubeFileType = YoutubeFileType.Mp3
        };
        var youtubeFile = new YoutubeFile();

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _youtubeService.GetMp3File(Arg.Any<string>()).Returns(youtubeFile);

        var handler = new DownloadLinkFeature.Handler(_authService, _playlistRepository, _youtubeService);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(youtubeFile, result);
        await _youtubeService.Received().GetMp3File(Arg.Any<string>());
        await _youtubeService.DidNotReceive().GetMp4File(Arg.Any<string>());
    }

    [Fact]
    public async Task DownloadLinkHandler_PublicPlaylist_ReturnsYoutubeFile()
    {
        var command = new DownloadLink.Command
        {
            Id = 1,
            YoutubeFileType = YoutubeFileType.Mp4
        };
        var youtubeFile = new YoutubeFile();

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);
        _youtubeService.GetMp4File(Arg.Any<string>()).Returns(youtubeFile);

        var handler = new DownloadLinkFeature.Handler(_authService, _playlistRepository, _youtubeService);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(youtubeFile, result);
        await _youtubeService.Received().GetMp4File(Arg.Any<string>());
        await _youtubeService.DidNotReceive().GetMp3File(Arg.Any<string>());
    }
}