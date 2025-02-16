namespace YoutubeLinks.Shared.Features.Users.Helpers;

public static class AuthConsts
{
    public static TimeSpan AccessTokenExpiry => TimeSpan.FromHours(6);

    public static TimeSpan RefreshTokenExpiry => TimeSpan.FromDays(7);

    public static TimeSpan FrontendTokenRefreshTime => TimeSpan.FromMinutes(5);
}