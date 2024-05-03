using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Api.Auth
{
    public interface IAuthenticator
    {
        JwtDto CreateTokens(User user);
    }

    public class Authenticator : IAuthenticator
    {
        private readonly IClock _clock;
        private readonly AuthOptions _options;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SigningCredentials _signingCredencials;
        private readonly JwtSecurityTokenHandler _jwtHandler = new();

        public Authenticator(
            IClock clock,
            IOptions<AuthOptions> options)
        {
            _clock = clock;
            _options = options.Value;
            _issuer = _options.Issuer;
            _audience = _options.Audience;
            _signingCredencials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
                SecurityAlgorithms.HmacSha256);
        }

        public JwtDto CreateTokens(User user)
        {
            return new()
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = GenerateRefreshToken(user),
            };
        }

        private string GenerateAccessToken(User user)
        {
            var now = _clock.Current();
            var expires = now.Add(AuthConsts.AccessTokenExpiry);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, Policy.User),
            };

            if (user.IsAdmin)
                claims.Add(new(ClaimTypes.Role, Policy.Admin));

            var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredencials);
            var accessToken = _jwtHandler.WriteToken(jwt);
            return accessToken;
        }

        private string GenerateRefreshToken(User user)
        {
            var now = _clock.Current();
            var expires = now.Add(AuthConsts.RefreshTokenExpiry);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredencials);
            var refreshToken = _jwtHandler.WriteToken(jwt);
            return refreshToken;
        }
    }
}
