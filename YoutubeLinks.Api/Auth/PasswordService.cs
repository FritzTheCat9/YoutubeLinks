using Microsoft.AspNetCore.Identity;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Auth;

public interface IPasswordService
{
    string Hash(string password);
    bool Validate(string password, string hashedPassword);
}

public class PasswordService(IPasswordHasher<User> passwordHasher) : IPasswordService
{
    public string Hash(string password)
        => passwordHasher.HashPassword(default!, password);

    public bool Validate(string password, string hashedPassword)
        => passwordHasher.VerifyHashedPassword(default!, hashedPassword, password)
            is PasswordVerificationResult.Success;
}