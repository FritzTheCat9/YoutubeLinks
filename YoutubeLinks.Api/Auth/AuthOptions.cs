namespace YoutubeLinks.Api.Auth
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public string FrontendUrl { get; set; }
    }
}
