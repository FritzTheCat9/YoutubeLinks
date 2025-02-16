using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.IntegrationTests.Features.Links.Queries;

public class GetLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetLink_ShouldReturnLink()
    {
        var userInfo = await LoginAsAdmin();
        var user = await GetUser(userInfo.UserId);
        var playlist = Playlist.Create("TestPlaylist", true, user);
        var link = playlist.AddLink("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ", "Rick Astley - Never Gonna Give You Up (Official Music Video)");

        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new GetLink.Query()
        {
            Id = link.Id,
        };

        var linkDto = await LinkApiClient.GetLink(command.Id);

        linkDto.Should().NotBeNull();
        linkDto.Should().Match<LinkDto>(x =>
            x.Id == link.Id &&
            x.Url == link.Url &&
            x.VideoId == link.VideoId &&
            x.Title == link.Title &&
            x.Downloaded == link.Downloaded &&
            x.PlaylistId == link.PlaylistId);
    }
}