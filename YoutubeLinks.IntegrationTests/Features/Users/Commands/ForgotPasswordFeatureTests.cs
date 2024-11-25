using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ForgotPasswordFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ForgotPassword_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = "Asd123!",
            EmailConfirmed = true,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new ForgotPassword.Command()
        {
            Email = user.Email,
        };

        await UserApiClient.ForgotPassword(command);

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.ForgotPasswordToken.Should().NotBeNull();
    }
}