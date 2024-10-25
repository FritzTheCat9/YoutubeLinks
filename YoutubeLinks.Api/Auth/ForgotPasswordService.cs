﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using YoutubeLinks.Api.Abstractions;

namespace YoutubeLinks.Api.Auth;

public interface IForgotPasswordService
{
    public string GenerateForgotPasswordToken(string email);
    public string GenerateForgotPasswordLink(string email, string token);
}

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly string _baseUrl;
    private readonly IClock _clock;
    private readonly AuthOptions _options;

    public ForgotPasswordService(
        IOptions<AuthOptions> options,
        IClock clock)
    {
        _options = options.Value;
        _clock = clock;
        _baseUrl = _options.FrontendUrl;
    }

    public string GenerateForgotPasswordToken(string email)
    {
        var tokenData = $"{email}_{_clock.Current().Ticks}_{_options.SigningKey}";

        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenData));
        var token = Convert.ToBase64String(hashedBytes);

        return token;
    }

    public string GenerateForgotPasswordLink(string email, string token)
    {
        return $"{_baseUrl}/resetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }
}