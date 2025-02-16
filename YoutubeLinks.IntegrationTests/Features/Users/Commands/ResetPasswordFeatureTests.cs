using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ResetPasswordFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResetPassword_ShouldSucceed_WhenDataIsValid()
    {
        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, false, false);
        user.SetPassword("Asd123!", PasswordService);
        user.SetForgotPasswordToken("Token");

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new ResetPassword.Command()
        {
            Email = "testuser@gmail.com",
            Token = user.ForgotPasswordToken,
            NewPassword = "Asd123!!",
            RepeatPassword = "Asd123!!",
        };

        var isPasswordReset = await UserApiClient.ResetPassword(command);
        isPasswordReset.Should().BeTrue();

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.PasswordHash.Should().NotBeNull();
        PasswordService.Validate(command.NewPassword, modifiedUser?.PasswordHash);
        modifiedUser?.ForgotPasswordToken.Should().BeNull();
    }
}