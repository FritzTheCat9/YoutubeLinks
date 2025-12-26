using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ForgotPasswordFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ForgotPassword_ShouldSucceed_WhenDataIsValid()
    {
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);
        user.SetPassword("Asd123!", PasswordService);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new ForgotPassword.Command()
        {
            Email = user.Email,
        };

        await UserApiClient.ForgotPassword(command);

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        Assert.NotNull(modifiedUser);
        Assert.NotNull(modifiedUser.ForgotPasswordToken);
    }
}