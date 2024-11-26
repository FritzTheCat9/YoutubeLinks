using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class RegisterFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Register_ShouldSucceed_WhenDataIsValid()
    {
        var command = new Register.Command()
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = "Asd123!",
            RepeatPassword = "Asd123!",
            ThemeColor = ThemeColor.System,
        };

        var userId = await UserApiClient.Register(command);

        var createdUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

        createdUser.Should().NotBeNull();
        createdUser.Should().Match<User>(x =>
            x.Id == userId &&
            x.Email == command.Email &&
            x.UserName == command.UserName &&
            PasswordService.Validate(command.Password, x.Password) &&
            x.ThemeColor == command.ThemeColor);
    }
}