namespace YoutubeLinks.Shared.Features.Users.Helpers;

public static class AuthConsts
{
    public static TimeSpan AccessTokenExpiry => TimeSpan.FromMinutes(15);

    public static TimeSpan RefreshTokenExpiry => TimeSpan.FromDays(1);

    public static TimeSpan FrontendTokenRefreshTime => TimeSpan.FromMinutes(5);
}