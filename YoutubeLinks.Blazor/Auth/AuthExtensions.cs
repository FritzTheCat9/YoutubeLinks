using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Blazor.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddCascadingAuthenticationState();

            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(Policy.User, x =>
                {
                    x.RequireClaim(ClaimTypes.Role, Policy.User);
                });
                config.AddPolicy(Policy.Admin, x =>
                {
                    x.RequireClaim(ClaimTypes.Role, Policy.Admin);
                });
            });

            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            return services;
        }
    }
}
