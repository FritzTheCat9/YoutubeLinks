namespace YoutubeLinks.Api.Auth;

public class AuthOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string SigningKey { get; init; }
    public string FrontendUrl { get; init; }
}