using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ResetPasswordFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResetPassword_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = false,
            ForgotPasswordToken = "Token",
            IsAdmin = false,
        };

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
        modifiedUser?.Password.Should().NotBeNull();
        PasswordService.Validate(command.NewPassword, modifiedUser?.Password);
        modifiedUser?.ForgotPasswordToken.Should().BeNull();
    }
}