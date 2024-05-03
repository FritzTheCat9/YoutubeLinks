using Microsoft.AspNetCore.Components.Authorization;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Blazor.Auth
{
    public class TokenRefreshService
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenRefreshService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            StartTokenRefreshTimer();
        }

        private void StartTokenRefreshTimer()
        {
            var timer = new System.Timers.Timer(AuthConsts.FrontendTokenRefreshTime);
            timer.Elapsed += async (sender, e) => await RefreshToken();
            timer.AutoReset = true;
            timer.Start();
        }

        private async Task RefreshToken()
        {
            using var scope = _serviceProvider.CreateScope();
            var userApiClient = scope.ServiceProvider.GetRequiredService<IUserApiClient>();
            var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
            var authenticationStateProvider = scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();

            try
            {
                var jwt = await jwtProvider.GetJwtDto();
                if (jwt == null || string.IsNullOrEmpty(jwt.RefreshToken))
                    return;

                var newJwt = await userApiClient.RefreshToken(new() { RefreshToken = jwt.RefreshToken });
                await jwtProvider.SetJwtDto(newJwt);

                var authStateProvider = authenticationStateProvider as AuthStateProvider;
                authStateProvider.NotifyAuthStateChanged();
            }
            catch (Exception)
            {
                Console.WriteLine("Error when refreshing jwt token");
            }
        }
    }
}
