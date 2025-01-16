using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Extensions;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Auth;

public static class AuthExtensions
{
    private const string SectionName = "Auth";

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetRequiredSection(SectionName));
        var authOptions = configuration.GetOptions<AuthOptions>(SectionName);

        services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Audience = authOptions.Audience;
                x.IncludeErrorDetails = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authOptions.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey))
                };
            });

        AddAuthorization(services);

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IPasswordService, PasswordService>();

        services.AddSingleton<IAuthenticator, Authenticator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    public static WebApplication UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.User, x => { x.RequireClaim(ClaimTypes.Role, Policy.User); })
            .AddPolicy(Policy.Admin, x => { x.RequireClaim(ClaimTypes.Role, Policy.Admin); });
    }
}