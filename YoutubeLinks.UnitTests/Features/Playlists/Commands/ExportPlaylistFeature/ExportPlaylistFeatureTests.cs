using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using YoutubeLinks.Shared.Features.Users.Helpers;
using ApiFeature = YoutubeLinks.Api.Features.Playlists.Commands.ExportPlaylistFeature.ExportPlaylistFeature;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands.ExportPlaylistFeature;

public class ExportPlaylistFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();

    [Fact]
    public async Task ExportPlaylistHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Json
        };

        _playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        var handler = new ApiFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ExportPlaylistHandler_ThrowsForbiddenException_IfPlaylistIsNotPublicAndNotOwnedByLoggedInUser()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Json
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("TestPlaylist", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new ApiFeature.Handler(_playlistRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ExportPlaylistHandler_ReturnsJSONPlaylistFile_IfPlaylistIsPublic()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Json
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("Name", true, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new ApiFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PlaylistFile>(result);
        Assert.Equal("application/json", result.ContentType);
        Assert.Equal("Name.json", result.FileName);
        Assert.Equal(PlaylistFileType.Json, result.PlaylistFileType);
    }

    [Fact]
    public async Task ExportPlaylistHandler_ReturnsTXTPlaylistFile_IfUserIsLoggedIn()
    {
        var command = new ExportPlaylist.Command
        {
            Id = 1,
            PlaylistFileType = PlaylistFileType.Txt
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        var playlist = Playlist.Create("Name", false, user);

        _playlistRepository.Get(Arg.Any<int>()).Returns(playlist);
        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        var handler = new ApiFeature.Handler(_playlistRepository, _authService);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PlaylistFile>(result);
        Assert.Equal("text/plain", result.ContentType);
        Assert.Equal("Name.txt", result.FileName);
        Assert.Equal(PlaylistFileType.Txt, result.PlaylistFileType);
    }
}