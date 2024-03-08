using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace YoutubeLinks.Blazor.Auth
{
    public interface IAuthService
    {
        Task<int?> GetCurrentUserId();
        Task<bool> IsLoggedInUser(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(AuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        public async Task<int?> GetCurrentUserId()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
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
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
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
    }
}
