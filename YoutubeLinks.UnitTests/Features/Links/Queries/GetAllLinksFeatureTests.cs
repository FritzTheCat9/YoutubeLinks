using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Links.Queries;

public class GetAllLinksFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetAllLinksHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var query = new GetAllLinks.Query { PlaylistId = 1, Downloaded = false };

        _playlistRepository.Get(1).Returns((Playlist)null);

        var handler = new GetAllLinksFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, default));
    }

    [Fact]
    public async Task GetAllLinksHandler_ReturnsLinks_ForOwnersPlaylist()
    {
        var query = new GetAllLinks.Query { PlaylistId = 1, Downloaded = false };

        var user = UserBuilder.Create().WithEmail("mail@mail.com").Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .WithLink("https://youtu.be/abc", "abc", "Test Video")
            .Build();

        var linksQueryable = playlist.Links.AsQueryable();

        _playlistRepository.Get(1).Returns(playlist);
        _authService.IsLoggedInUser(playlist.UserId).Returns(true);
        _playlistRepository.GetPlaylistLinksAsQueryable(1, true).Returns(linksQueryable);

        var handler = new GetAllLinksFeature.Handler(_playlistRepository, _authService);
        var result = (await handler.Handle(query, default)).ToList();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.IsType<GetAllLinks.LinkInfoDto>(result.First());
    }

    [Fact]
    public async Task GetAllLinksHandler_ReturnsLinks_ForPublicPlaylist_WhenUserNotOwner()
    {
        var query = new GetAllLinks.Query { PlaylistId = 1, Downloaded = false };

        var user = UserBuilder.Create().WithEmail("owner@mail.com").Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .Public(true)
            .WithLink("https://youtu.be/xyz", "xyz", "Public Video")
            .Build();

        var linksQueryable = playlist.Links.AsQueryable();

        _playlistRepository.Get(1).Returns(playlist);
        _authService.IsLoggedInUser(playlist.UserId).Returns(false);
        _playlistRepository.GetPlaylistLinksAsQueryable(1, false).Returns(linksQueryable);

        var handler = new GetAllLinksFeature.Handler(_playlistRepository, _authService);
        var result = (await handler.Handle(query, default)).ToList();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.IsType<GetAllLinks.LinkInfoDto>(result.First());
    }
}
