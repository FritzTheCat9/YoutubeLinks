using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Playlists.Commands;

public class ResetLinksDownloadedFlagFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResetLinksDownloadedFlag_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();

        var links = new List<Link>
        {
            new()
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                VideoId = "dQw4w9WgXcQ",
                Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                Downloaded = false,
            },
            new()
            {
                Url = "https://www.youtube.com/watch?v=GtUVQei3nX4",
                VideoId = "GtUVQei3nX4",
                Title = "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams",
                Downloaded = false,
            }
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.AddRange(links);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new ResetLinksDownloadedFlag.Command()
        {
            Id = playlist.Id,
            IsDownloaded = true,
        };

        await PlaylistApiClient.ResetLinksDownloadedFlag(command);

        var updatedPlaylist = await Context.Playlists
            .AsNoTracking()
            .Include(x => x.Links)
            .FirstOrDefaultAsync(x => x.Id == playlist.Id);
        
        updatedPlaylist.Should().NotBeNull();
        updatedPlaylist?.Links.Should().HaveCount(2);
        updatedPlaylist?.Links.Should().OnlyContain(link => link.Downloaded == true);
    }

    [Fact]
    public async Task ResetLinksDownloadedFlag_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = await LoginAsAdmin();

        var links = new List<Link>
        {
            new()
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                VideoId = "dQw4w9WgXcQ",
                Title = "Rick Astley - Never Gonna Give You Up (Official Music Video)",
                Downloaded = false,
            },
            new()
            {
                Url = "https://www.youtube.com/watch?v=GtUVQei3nX4",
                VideoId = "GtUVQei3nX4",
                Title = "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams",
                Downloaded = false,
            }
        };

        var playlist = new Playlist { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        playlist.Links.AddRange(links);

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        var command = new ResetLinksDownloadedFlag.Command()
        {
            Id = playlist.Id,
            IsDownloaded = true,
        };

        await Logout();

        await FluentActions.Invoking(() => PlaylistApiClient.ResetLinksDownloadedFlag(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}