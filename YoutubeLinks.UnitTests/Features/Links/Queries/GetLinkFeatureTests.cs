using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Queries;

public class GetLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
    {
        var query = new GetLink.Query
        {
            Id = 1
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUserAndPlaylistIsNotPublic()
    {
        var query = new GetLink.Query
        {
            Id = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetLinkHandler_ReturnsLinkDto_IfPlaylistIsPublic()
    {
        var query = new GetLink.Query
        {
            Id = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<LinkDto>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLinkHandler_ReturnsLinkDto_IfPlaylistIsOwnedByLoggedInUser()
    {
        var query = new GetLink.Query
        {
            Id = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<LinkDto>(result);
        Assert.NotNull(result);
    }
}