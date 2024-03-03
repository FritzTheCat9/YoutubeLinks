using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Auth
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
                throw new MyUnauthorizedException();

            if (authorizeResult.Forbidden)
                throw new MyForbiddenException();

            await next(context);
        }
    }
}
