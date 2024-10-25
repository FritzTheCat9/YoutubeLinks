using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using YoutubeLinks.Api.Abstractions;

namespace YoutubeLinks.Api.Auth;

public interface IEmailConfirmationService
{
    public string GenerateEmailConfirmationToken(string email);
    public string GenerateConfirmationLink(string email, string token);
}

public class EmailConfirmationService : IEmailConfirmationService
{
    private readonly string _baseUrl;
    private readonly IClock _clock;
    private readonly AuthOptions _options;

    public EmailConfirmationService(
        IOptions<AuthOptions> options,
        IClock clock)
    {
        _options = options.Value;
        _clock = clock;
        _baseUrl = _options.FrontendUrl;
    }

    public string GenerateEmailConfirmationToken(string email)
    {
        var tokenData = $"{email}_{_clock.Current().Ticks}_{_options.SigningKey}";

        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenData));
        var token = Convert.ToBase64String(hashedBytes);

        return token;
    }

    public string GenerateConfirmationLink(string email, string token) 
        => $"{_baseUrl}/confirmEmail?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
}