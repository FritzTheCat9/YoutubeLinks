using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ResendConfirmationEmailFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResendConfirmationEmail_ShouldSucceed_WhenDataIsValid()
    {
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, false, false);
        user.SetPassword("Asd123!", PasswordService);

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new ResendConfirmationEmail.Command()
        {
            Email = "testuser@gmail.com",
        };

        await UserApiClient.ResendConfirmationEmail(command);

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.EmailConfirmationToken.Should().NotBeNull();
    }
}