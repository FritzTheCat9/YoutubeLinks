using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Links.Commands;

public class CreateLinkFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateLink_ShouldSucceed_WhenDataIsValid()
    {
        var user = await LoginAsAdmin();
        
        var playlist = new Playlist() { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();
        
        var command = new CreateLink.Command()
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = playlist.Id,
        };

        var linkId = await LinkApiClient.CreateLink(command);
        
        var link = await Context.Links.FirstOrDefaultAsync(x => x.Id == linkId);
        
        link.Should().NotBeNull();
        link!.Url.Should().Be(command.Url);
        link.PlaylistId.Should().Be(command.PlaylistId);
        link.Downloaded.Should().BeFalse();
        link.VideoId.Should().Be("dQw4w9WgXcQ");
    }
    
    [Fact]
    public async Task CreateLink_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = await LoginAsAdmin();
        
        var playlist = new Playlist() { Name = "TestPlaylist", Public = true, UserId = user.UserId };
        await Context.Playlists.AddAsync(playlist);
        await Context.SaveChangesAsync();

        await Logout();
        
        var command = new CreateLink.Command()
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = playlist.Id,
        };
        
        try
        {
            await LinkApiClient.CreateLink(command);
        }
        catch (Exception e)
        {
            e.Should().BeOfType<MyUnauthorizedException>();
        }
    }
}