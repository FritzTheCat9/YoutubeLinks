using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Auth;

public interface IAuthService
{
    Task<int?> GetCurrentUserId();
    Task<bool> IsLoggedInUser(int userId);
    Task Login(JwtDto token, string redirectUrl = null);
    Task Logout(string redirectUrl = null);
    Task RefreshToken();
}

public class AuthService(
    AuthenticationStateProvider stateProvider,
    IJwtProvider jwtProvider,
    IUserApiClient userApiClient,
    NavigationManager navigationManager)
    : IAuthService
{
    public async Task<int?> GetCurrentUserId()
    {
        if (stateProvider is not AuthStateProvider authStateProvider)
            return null;

        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!(user.Identity?.IsAuthenticated ?? false))
            return null;

        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdString == null)
            return null;

        if (!int.TryParse(userIdString, out var userIdInt))
            return null;

        return userIdInt;
    }

    public async Task<bool> IsLoggedInUser(int userId)
    {
        if (stateProvider is not AuthStateProvider authStateProvider)
            return false;

        var authState = await authStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        if (!(user.Identity?.IsAuthenticated ?? false))
            return false;
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdString == null)
            return false;

        if (!int.TryParse(userIdString, out var userIdInt))
            return false;

        return userId == userIdInt;
    }

    public async Task Login(JwtDto token, string redirectUrl = null)
    {
        await jwtProvider.SetJwtDto(token);

        var authStateProvider = stateProvider as AuthStateProvider;
        authStateProvider?.NotifyAuthStateChanged();

        if (!string.IsNullOrEmpty(redirectUrl))
            navigationManager.NavigateTo(redirectUrl);
    }

    public async Task Logout(string redirectUrl = null)
    {
        await jwtProvider.RemoveJwtDto();

        var authStateProvider = stateProvider as AuthStateProvider;
        authStateProvider?.NotifyAuthStateChanged();

        if (!string.IsNullOrEmpty(redirectUrl))
            navigationManager.NavigateTo(redirectUrl);
    }

    public async Task RefreshToken()
    {
        try
        {
            var jwt = await jwtProvider.GetJwtDto();
            if (jwt is null || string.IsNullOrEmpty(jwt.RefreshToken))
            {
                await Logout();
            }
            else
            {
                var newJwt = await userApiClient.RefreshToken(new RefreshToken.Command
                    { RefreshToken = jwt.RefreshToken });
                await Login(newJwt);
            }
        }
        catch (Exception)
        {
            await Logout();
        }
    }
}