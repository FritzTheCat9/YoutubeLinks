using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace YoutubeLinks.Blazor.Auth
{
    public interface IAuthService
    {
        Task<int?> GetCurrentUserId();
    }

    public class AuthService : IAuthService
    {
        private readonly Task<AuthenticationState> _authenticationStateTask;

        public AuthService(AuthenticationStateProvider authStateProvider)
        {
            _authenticationStateTask = authStateProvider.GetAuthenticationStateAsync();
        }

        public async Task<int?> GetCurrentUserId()
        {
            var authState = await _authenticationStateTask;
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
    }
}
