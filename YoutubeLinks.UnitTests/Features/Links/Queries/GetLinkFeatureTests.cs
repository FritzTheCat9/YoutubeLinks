using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using YoutubeLinks.UnitTests.Builders;

namespace YoutubeLinks.UnitTests.Features.Links.Queries;

public class GetLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task GetLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
    {
        var query = new GetLink.Query { Id = 1 };

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>())
            .Returns(Task.FromResult<Playlist>(null));

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByUserAndNotPublic()
    {
        var query = new GetLink.Query { Id = 1 };

        var user = UserBuilder.Create().WithEmail("testuser@gmail.com").Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .Public(false)
            .WithLink("https://youtu.be/test", "test", "Test Video")
            .Build();

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetLinkHandler_ReturnsLinkDto_IfPlaylistIsPublic()
    {
        var query = new GetLink.Query { Id = 1 };

        var user = UserBuilder.Create().Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .Public(true)
            .WithLink("https://youtu.be/public", "public", "Public Video")
            .Build();

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<LinkDto>(result);
        Assert.Equal("Public Video", result.Title);
    }

    [Fact]
    public async Task GetLinkHandler_ReturnsLinkDto_IfPlaylistIsOwnedByUser()
    {
        var query = new GetLink.Query { Id = 1 };

        var user = UserBuilder.Create().Build();
        var playlist = PlaylistBuilder.Create()
            .WithUser(user)
            .Public(false)
            .WithLink("https://youtu.be/owned", "owned", "Owned Video")
            .Build();

        _playlistRepository.FindPlaylistContainingLink(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(user.Id).Returns(true);

        var handler = new GetLinkFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<LinkDto>(result);
        Assert.Equal("Owned Video", result.Title);
    }
}
