using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Playlists.Queries;

public class GetPlaylistFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetPlaylistHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var query = new GetPlaylist.Query { Id = 1 };
        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new GetPlaylistFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetPlaylistHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUserAndPlaylistIsNotPublic()
    {
        var query = new GetPlaylist.Query { Id = 1 };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(false); // check ownership

        var handler = new GetPlaylistFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetPlaylistHandler_ReturnsPlaylistDto_IfPlaylistIsPublic()
    {
        var query = new GetPlaylist.Query { Id = 1 };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(false); // not owner, but public

        var handler = new GetPlaylistFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<PlaylistDto>(result);
        Assert.NotNull(result);
        Assert.Equal(playlist.Name, result.Name);
    }

    [Fact]
    public async Task GetPlaylistHandler_ReturnsPlaylistDto_IfPlaylistIsOwnedByLoggedInUser()
    {
        var query = new GetPlaylist.Query { Id = 1 };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true); // owner

        var handler = new GetPlaylistFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<PlaylistDto>(result);
        Assert.NotNull(result);
        Assert.Equal(playlist.Name, result.Name);
    }
}
