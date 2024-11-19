using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests.Features.Users;

public class LoginFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    private const string Url = "/api/users/login";

    [Fact]
    public async Task GIVEN_WHEN_THEN()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp@gmail.com", Password = "Asd123!"
        };
        
        var response = await Client.PostAsJsonAsync($"{Url}", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        Context.Users.Count().Should().Be(2);
    }
    
    [Fact]
    public async Task GIVEN_WHEN_THEN2()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp@gmail.com", Password = "Asd123!!"
        };
        
        var response = await Client.PostAsJsonAsync($"{Url}", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Login_ShouldReturnJwtToken()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp@gmail.com", Password = "Asd123!"
        };

        var result = await UserApiClient.Login(command);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }
}