using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Queries;

public class GetAllLinksFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetAllLinksHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var query = new GetAllLinks.Query
        {
            PlaylistId = 1,
            Downloaded = false
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new GetAllLinksFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllLinksHandler_ReturnsLinkInfoDtos_IfPlaylistIsOwnedByUser()
    {
        var query = new GetAllLinks.Query
        {
            PlaylistId = 1,
            Downloaded = false
        };

        var links = new List<Link>
        {
            new()
            {
                Id = 1
            }
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
        {
            UserId = 1
        });
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _playlistRepository.AsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(links.AsQueryable());

        var handler = new GetAllLinksFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<List<GetAllLinks.LinkInfoDto>>(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllLinksHandler_ReturnsLinkInfoDtos_IfPlaylistIsNotOwnedByUser()
    {
        var query = new GetAllLinks.Query
        {
            PlaylistId = 1,
            Downloaded = false
        };

        var links = new List<Link>
        {
            new()
            {
                Id = 1
            }
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);
        _playlistRepository.GetPlaylistLinksAsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(links.AsQueryable());

        var handler = new GetAllLinksFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<List<GetAllLinks.LinkInfoDto>>(result);
        Assert.Single(result);
    }
}