using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class ResendConfirmationEmailFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResendConfirmationEmail_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = false,
            IsAdmin = false,
        };

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