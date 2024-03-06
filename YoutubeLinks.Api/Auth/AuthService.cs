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
        private readonly ClaimsPrincipal _user;

        public ClaimsPrincipal User => _user;

        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext is null)
                throw new MyServerException();

            _user = httpContextAccessor.HttpContext.User;
        }

        public bool IsInRole(string roleName)
            => _user.IsInRole(roleName);

        public bool IsInAnyRole(params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (_user.IsInRole(roleName))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsLoggedInUser(int userId)
        {
            var userIdString = _user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null)
                return false;

            if (!int.TryParse(userIdString, out int userIdInt))
                return false;

            if (userId != userIdInt)
                return false;

            return true;
        }

        public int? GetCurrentUserId()
        {
            if (_user.Identity?.IsAuthenticated ?? false)
            {
                var userIdString = _user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
