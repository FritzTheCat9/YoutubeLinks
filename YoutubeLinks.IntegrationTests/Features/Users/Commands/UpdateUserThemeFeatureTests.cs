using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class UpdateUserThemeFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateUserTheme_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser1@gmail.com",
            UserName = "TestUser1",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = true,
            IsAdmin = false,
            ThemeColor = ThemeColor.Light,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new UpdateUserTheme.Command()
        {
            Id = user.Id,
            ThemeColor = ThemeColor.Dark,
        };

        await LoginAsUser(user.Email, "Asd123!");
        
        await UserApiClient.UpdateUserTheme(command);

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.ThemeColor.Should().Be(ThemeColor.Dark);
    }
    
    
    [Fact]
    public async Task UpdateUserTheme_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = new User
        {
            Email = "testuser2@gmail.com",
            UserName = "TestUser2",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = true,
            IsAdmin = false,
            ThemeColor = ThemeColor.Light,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new UpdateUserTheme.Command()
        {
            Id = user.Id,
            ThemeColor = ThemeColor.Dark,
        };

        await Logout();

        await FluentActions.Invoking(() => UserApiClient.UpdateUserTheme(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}