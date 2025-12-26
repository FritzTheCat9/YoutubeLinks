using Microsoft.EntityFrameworkCore;
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

        Assert.NotNull(createdUser);
        Assert.Equal(userId, createdUser.Id);
        Assert.Equal(command.Email, createdUser.Email);
        Assert.Equal(command.UserName, createdUser.UserName);
        Assert.True(PasswordService.Validate(command.Password, createdUser.PasswordHash));
        Assert.Equal(command.ThemeColor, createdUser.ThemeColor);
    }
}