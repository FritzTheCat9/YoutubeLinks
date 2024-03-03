using System.Security.Claims;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Auth
{
    public interface IAuthService
    {
        bool IsInRole(string roleName);
        bool IsInAnyRole(params string[] roleNames);
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
    }
}
