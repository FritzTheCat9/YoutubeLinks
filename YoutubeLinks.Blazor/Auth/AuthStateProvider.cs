﻿using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJwtProvider _jwtProvider;

        public AuthStateProvider(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jwtProvider.GetJwtDto();
            var authenticationState = new AuthenticationState(new ClaimsPrincipal());

            if (token == null || string.IsNullOrEmpty(token.AccessToken))
                return authenticationState;

            var tokenHandler = new JwtSecurityTokenHandler();

            var isTokenValid = tokenHandler.CanReadToken(token.AccessToken);
            if (!isTokenValid)
                return authenticationState;

            try
            {
                if (tokenHandler.ReadToken(token.AccessToken) is JwtSecurityToken jsonToken)
                {
                    if (jsonToken.ValidTo < DateTime.UtcNow)
                        return authenticationState;

                    return new AuthenticationState(new ClaimsPrincipal(
                        new ClaimsIdentity(jsonToken.Claims,
                                           Jwt.AuthenticationType,
                                           ClaimsIdentity.DefaultNameClaimType,
                                           ClaimsIdentity.DefaultRoleClaimType)));
                }
            }
            catch (Exception)
            {
                // TODO: log exception to file (ReadToken method exception)
                return authenticationState;
            }

            return authenticationState;
        }

        public void NotifyAuthStateChanged()
            => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
