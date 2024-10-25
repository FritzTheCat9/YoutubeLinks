namespace YoutubeLinks.Shared.Features.Users.Responses
{
    public class JwtDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public static class Jwt
    {
        public const string Dto = "JwtDto";
        public const string AuthenticationType = "Jwt";
    }
}
