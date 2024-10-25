using YoutubeLinks.Shared.Features.Users.Helpers;
using Timer = System.Timers.Timer;

namespace YoutubeLinks.Blazor.Auth;

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
        var timer = new Timer(AuthConsts.FrontendTokenRefreshTime);
        timer.Elapsed += async (sender, e) => await RefreshToken();
        timer.AutoReset = true;
        timer.Start();
    }

    public async Task RefreshToken()
    {
        using var scope = _serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        await authService.RefreshToken();
    }
}