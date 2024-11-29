using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Users.Queries;

public class GetUserFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetUser_ShouldReturnUser()
    {
        var user = new User
        {
            Email = "testuser@gmail.com",
            UserName = "TestUser",
            Password = PasswordService.Hash("Asd123!"),
            EmailConfirmed = true,
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var command = new GetUser.Query()
        {
            Id = user.Id
        };

        await Logout();

        var userDto = await UserApiClient.GetUser(command.Id);

        userDto.Should().NotBeNull();
        userDto.Id.Should().Be(user.Id);
        userDto.Email.Should().Be(user.Email);
        userDto.UserName.Should().Be(user.UserName);
    }
}