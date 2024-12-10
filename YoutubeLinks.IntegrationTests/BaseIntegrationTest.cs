using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Sdk.Clients;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.IntegrationTests;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
        IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly AppDbContext Context;
    protected readonly HttpClient Client;
    protected readonly IApiClient ApiClient;
    protected readonly IUserApiClient UserApiClient;
    protected readonly IPlaylistApiClient PlaylistApiClient;
    protected readonly ILinkApiClient LinkApiClient;
    protected readonly IJwtProvider JwtProvider;
    protected readonly IPasswordService PasswordService;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Client = factory.CreateClient();
        ApiClient = _scope.ServiceProvider.GetRequiredService<IApiClient>();
        UserApiClient = _scope.ServiceProvider.GetRequiredService<IUserApiClient>();
        PlaylistApiClient = _scope.ServiceProvider.GetRequiredService<IPlaylistApiClient>();
        LinkApiClient = _scope.ServiceProvider.GetRequiredService<ILinkApiClient>();
        JwtProvider = _scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        PasswordService = _scope.ServiceProvider.GetRequiredService<IPasswordService>();
    }

    protected async Task<UserInfo> LoginAsAdmin()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp@gmail.com", Password = "Asd123!"
        };

        var jwt = await UserApiClient.Login(command);

        await JwtProvider.SetJwtDto(jwt);

        jwt.Should().NotBeNull();
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();

        var userInfo = await GetUserInfo();
        return userInfo;
    }

    protected async Task<UserInfo> LoginAsUser()
    {
        var command = new Login.Command
        {
            Email = "ytlinksapp1@gmail.com", Password = "Asd123!"
        };

        var jwt = await UserApiClient.Login(command);

        await JwtProvider.SetJwtDto(jwt);

        jwt.Should().NotBeNull();
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();

        var userInfo = await GetUserInfo();
        return userInfo;
    }
    
    protected async Task<UserInfo> LoginAsUser(string email, string password)
    {
        var command = new Login.Command
        {
            Email = email, Password = password
        };

        var jwt = await UserApiClient.Login(command);

        await JwtProvider.SetJwtDto(jwt);

        jwt.Should().NotBeNull();
        jwt.AccessToken.Should().NotBeNull();
        jwt.RefreshToken.Should().NotBeNull();

        var userInfo = await GetUserInfo();
        return userInfo;
    }

    protected async Task Logout()
    {
        await JwtProvider.RemoveJwtDto();
    }

    protected class UserInfo
    {
        public int UserId { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required List<string> Roles { get; init; }
        public required string RefreshToken { get; init; }
    }

    private async Task<UserInfo> GetUserInfo()
    {
        var jwt = await JwtProvider.GetJwtDto();

        if (jwt == null || string.IsNullOrEmpty(jwt.AccessToken))
        {
            throw new MyUnauthorizedException();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var isTokenValid = tokenHandler.CanReadToken(jwt.AccessToken);
        if (!isTokenValid)
        {
            throw new MyUnauthorizedException();
        }

        var jsonToken = tokenHandler.ReadToken(jwt.AccessToken) as JwtSecurityToken;

        var userIdString = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userIdString == null)
        {
            throw new MyUnauthorizedException();
        }

        if (!int.TryParse(userIdString, out var userId))
        {
            throw new MyUnauthorizedException();
        }

        var userName = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var roles = jsonToken?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            roles is null ||
            roles.Count == 0)
        {
            throw new MyUnauthorizedException();
        }

        return new UserInfo
        {
            UserId = userId,
            UserName = userName,
            Email = email,
            Roles = roles,
            RefreshToken = jwt.RefreshToken,
        };
    }

    public void Dispose()
    {
        _scope.Dispose();
        Context.Dispose();
        Client.Dispose();
    }
}