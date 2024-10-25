namespace YoutubeLinks.Api.Emails;

public class EmailOptions
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string SmtpHost { get; init; }
    public int Port { get; init; }
    public bool SendEmails { get; init; }
}