namespace YoutubeLinks.Shared.Features.Users.Responses
{
    public class JwtDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public static class Jwt
    {
        public static readonly string Dto = "JwtDto";
        public static readonly string AuthnticationType = "Jwt";
    }
}
