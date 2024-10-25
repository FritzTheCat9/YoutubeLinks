namespace YoutubeLinks.Api.Emails
{
    public class EmailOptions
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public bool SendEmails { get; set; }
    }
}
