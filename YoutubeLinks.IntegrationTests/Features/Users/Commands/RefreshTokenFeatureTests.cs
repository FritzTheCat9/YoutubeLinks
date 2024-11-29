using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users.Commands;

public class RefreshTokenFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RefreshToken_ShouldSucceed_WhenDataIsValid()
    {
        var user = new User
        {
            Email = "testuser1@gmail.com",
            UserName = "TestUser1",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = true,
            IsAdmin = false,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var userInfo = await LoginAsUser(user.Email, "Asd123!");

        var command = new RefreshToken.Command()
        {
            RefreshToken = userInfo.RefreshToken,
        };

        var jwt = await UserApiClient.RefreshToken(command);
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();

        var modifiedUser = await Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);

        modifiedUser.Should().NotBeNull();
        modifiedUser?.RefreshToken.Should().NotBeNull();
        modifiedUser?.RefreshToken.Should().Be(jwt.RefreshToken);
    }
    
    [Fact]
    public async Task RefreshToken_ShouldThrowUnauthorizedException_WhenUserIsNotLoggedIn()
    {
        var user = new User
        {
            Email = "testuser2@gmail.com",
            UserName = "TestUser2",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = true,
            IsAdmin = false,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var userInfo = await LoginAsUser(user.Email, "Asd123!");

        var command = new RefreshToken.Command()
        {
            RefreshToken = userInfo.RefreshToken,
        };

        await Logout();

        await FluentActions.Invoking(() => UserApiClient.RefreshToken(command))
            .Should().ThrowAsync<MyUnauthorizedException>();
    }
}