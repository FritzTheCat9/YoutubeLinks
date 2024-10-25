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
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _jwtHandler = new();

        public Authenticator(
            IClock clock,
            IOptions<AuthOptions> options)
        {
            var authOptions = options.Value;
            
            _clock = clock;
            _issuer = authOptions.Issuer;
            _audience = authOptions.Audience;
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256);
        }

        public JwtDto CreateTokens(User user)
        {
            return new JwtDto
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
                claims.Add(new Claim(ClaimTypes.Role, Policy.Admin));

            var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredentials);
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

            var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires, _signingCredentials);
            var refreshToken = _jwtHandler.WriteToken(jwt);
            return refreshToken;
        }
    }
}
