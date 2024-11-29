using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ConfirmEmailFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ConfirmEmail_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = false,
            EmailConfirmationToken = "TEST_TOKEN",
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        
        var command = new ConfirmEmail.Command()
        {
            Email = user.Email,
            Token = user.EmailConfirmationToken,
        };

        var isEmailConfirmed = await UserApiClient.ConfirmEmail(command);
        isEmailConfirmed.Should().BeTrue();

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.EmailConfirmed.Should().BeTrue();
        modifiedUser?.EmailConfirmationToken.Should().BeNull();
    }
}