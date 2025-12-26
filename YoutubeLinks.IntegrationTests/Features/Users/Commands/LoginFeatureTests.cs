using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class LoginFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_ShouldSucceed_WhenAdminCredentialsAreValid()
    {
        await LoginAsAdmin();
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenUserCredentialsAreValid()
    {
        await LoginAsUser();
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenDataIsValid()
    {
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        user.SetPassword("Asd123!", PasswordService);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new Login.Command()
        {
            Email = user.Email,
            Password = "Asd123!",
        };

        var jwt = await UserApiClient.Login(command);
        Assert.NotNull(jwt);
        Assert.NotNull(jwt.AccessToken);
        Assert.NotNull(jwt.RefreshToken);

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        Assert.NotNull(modifiedUser);
        Assert.NotNull(modifiedUser.RefreshToken);
        Assert.Equal(jwt.RefreshToken, modifiedUser.RefreshToken);
    }
}