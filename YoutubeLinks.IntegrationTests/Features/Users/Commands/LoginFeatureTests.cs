using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class LoginFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_ShouldSucceed_WhenAdminCredentialsAreValid()
    {
        await LoginAsAdmin();
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenUserCredentialsAreValid()
    {
        await LoginAsUser();
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==",
            EmailConfirmed = true,
            IsAdmin = false,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new Login.Command()
        {
            Email = user.Email,
            Password = "Asd123!",
        };

        var jwt = await UserApiClient.Login(command);
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.RefreshToken.Should().NotBeNull();
        modifiedUser?.RefreshToken.Should().Be(jwt.RefreshToken);
    }
}