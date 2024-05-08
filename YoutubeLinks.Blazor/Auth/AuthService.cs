using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Auth
{
    public interface IAuthService
    {
        Task<int?> GetCurrentUserId();
        Task<bool> IsLoggedInUser(int userId);
        Task Login(JwtDto token, string redirectUrl = null);
        Task Logout(string redirectUrl = null);
        Task RefreshToken();
    }

    public class AuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserApiClient _userApiClient;
        private readonly NavigationManager _navigationManager;

        public AuthService(
            AuthenticationStateProvider authStateProvider,
            IJwtProvider jwtProvider,
            IUserApiClient userApiClient,
            NavigationManager navigationManager)
        {
            _authStateProvider = authStateProvider;
            _jwtProvider = jwtProvider;
            _userApiClient = userApiClient;
            _navigationManager = navigationManager;
        }

        public async Task<int?> GetCurrentUserId()
        {
            var authStateProvider = (_authStateProvider as AuthStateProvider);
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdString == null)
                    return null;

                if (!int.TryParse(userIdString, out int userIdInt))
                    return null;

                return userIdInt;
            }

            return null;
        }

        public async Task<bool> IsLoggedInUser(int userId)
        {
            var authStateProvider = (_authStateProvider as AuthStateProvider);
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdString == null)
                    return false;

                if (!int.TryParse(userIdString, out int userIdInt))
                    return false;

                if (userId != userIdInt)
                    return false;

                return true;
            }

            return false;
        }

        public async Task Login(JwtDto token, string redirectUrl = null)
        {
            await _jwtProvider.SetJwtDto(token);

            var authStateProvider = (_authStateProvider as AuthStateProvider);
            authStateProvider.NotifyAuthStateChanged();

            if (!string.IsNullOrEmpty(redirectUrl))
                _navigationManager.NavigateTo(redirectUrl);
        }

        public async Task Logout(string redirectUrl = null)
        {
            await _jwtProvider.RemoveJwtDto();

            var authStateProvider = (_authStateProvider as AuthStateProvider);
            authStateProvider.NotifyAuthStateChanged();

            if (!string.IsNullOrEmpty(redirectUrl))
                _navigationManager.NavigateTo(redirectUrl);
        }

        public async Task RefreshToken()
        {
            try
            {
                var jwt = await _jwtProvider.GetJwtDto();
                if (jwt == null || string.IsNullOrEmpty(jwt.RefreshToken))
                    await Logout();

                var newJwt = await _userApiClient.RefreshToken(new() { RefreshToken = jwt.RefreshToken });
                await Login(newJwt);
            }
            catch (Exception)
            {
                await Logout();
            }
        }
    }
}
