using FluentAssertions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users;

public class LoginFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldLoginAsAdmin()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp@gmail.com", Password = "Asd123!"
        };

        var jwt = await UserApiClient.Login(command);

        jwt.Should().NotBeNull();
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Login_WithValidCredentials_ShouldLoginAsUser()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp1@gmail.com", Password = "Asd123!"
        };

        var jwt = await UserApiClient.Login(command);

        jwt.Should().NotBeNull();
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();
    }
}