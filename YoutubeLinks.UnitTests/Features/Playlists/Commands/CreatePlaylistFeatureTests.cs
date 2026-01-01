using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands;

public class CreatePlaylistFeatureTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task CreatePlaylistHandler_ThrowsForbiddenException_IfUserIsNotAuthenticated()
    {
        var command = new CreatePlaylist.Command
        {
            Name = "Name",
            Public = true
        };

        _authService.GetCurrentUserId().Returns((int?)null);

        var handler = new CreatePlaylistFeature.Handler(_userRepository, _playlistRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreatePlaylistHandler_ReturnsCreatedPlaylistId()
    {
        var command = new CreatePlaylist.Command
        {
            Name = "Name",
            Public = true
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);

        _authService.GetCurrentUserId().Returns(123);
        _playlistRepository.Create(Arg.Any<Playlist>()).Returns(1);
        _userRepository.Get(Arg.Any<int>()).Returns(user);

        var handler = new CreatePlaylistFeature.Handler(_userRepository, _playlistRepository, _authService);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result);
        await _playlistRepository.Received().Create(Arg.Any<Playlist>());
    }
}