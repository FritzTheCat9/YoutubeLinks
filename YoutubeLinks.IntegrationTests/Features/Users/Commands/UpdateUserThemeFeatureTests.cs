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
        var user = User.Create("testuser1@gmail.com", "TestUser1", ThemeColor.Light, false, true);
        user.SetPassword("Asd123!", PasswordService);
        user.SetForgotPasswordToken("Token");

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

        Assert.NotNull(modifiedUser);
        Assert.Equal(ThemeColor.Dark, modifiedUser.ThemeColor);
    }

    [Fact]
    public async Task UpdateUserTheme_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = User.Create("testuser2@gmail.com", "TestUser2", ThemeColor.Light, false, true);
        user.SetPassword("Asd123!", PasswordService);
        user.SetForgotPasswordToken("Token");

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new UpdateUserTheme.Command()
        {
            Id = user.Id,
            ThemeColor = ThemeColor.Dark,
        };

        await Logout();

        await Assert.ThrowsAsync<MyUnauthorizedException>(async () =>
        {
            await UserApiClient.UpdateUserTheme(command);
        });
    }
}