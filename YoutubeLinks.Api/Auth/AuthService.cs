using System.Security.Claims;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Auth
{
    public interface IAuthService
    {
        bool IsInRole(string roleName);
        bool IsInAnyRole(params string[] roleNames);
        bool IsLoggedInUser(int userId);
        int? GetCurrentUserId();

        ClaimsPrincipal User { get; }
    }

    public class AuthService : IAuthService
    {
        public ClaimsPrincipal User { get; }

        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext is null)
                throw new MyServerException();

            User = httpContextAccessor.HttpContext.User;
        }

        public bool IsInRole(string roleName)
            => User.IsInRole(roleName);

        public bool IsInAnyRole(params string[] roleNames) 
            => roleNames.Any(roleName => User.IsInRole(roleName));

        public bool IsLoggedInUser(int userId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null)
                return false;

            if (!int.TryParse(userIdString, out var userIdInt))
                return false;

            return userId == userIdInt;
        }

        public int? GetCurrentUserId()
        {
            if (!(User.Identity?.IsAuthenticated ?? false)) 
                return null;
            
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null)
                return null;

            if (!int.TryParse(userIdString, out var userIdInt))
                return null;

            return userIdInt;

        }
    }
}
