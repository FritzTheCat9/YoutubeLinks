namespace YoutubeLinks.Shared.Features.Users.Responses;

public class JwtDto
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
}

public static class Jwt
{
    public const string Dto = "JwtDto";
    public const string AuthenticationType = "Jwt";
}