using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ConfirmEmailFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ConfirmEmail_ShouldSucceed_WhenDataIsValid()
    {
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, false);
        user.SetPassword("Asd123!", PasswordService);
        user.SetEmailConfirmationToken("TEST_TOKEN");

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new ConfirmEmail.Command()
        {
            Email = user.Email,
            Token = user.EmailConfirmationToken,
        };

        var isEmailConfirmed = await UserApiClient.ConfirmEmail(command);
        Assert.True(isEmailConfirmed);

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        Assert.NotNull(modifiedUser);
        Assert.True(modifiedUser.EmailConfirmed);
        Assert.Null(modifiedUser.EmailConfirmationToken);
    }
}