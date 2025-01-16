using YoutubeLinks.Api.Auth;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Data.Entities;

public class User : Entity, IAggregateRoot
{
    private readonly List<Playlist> _playlists = [];
    public IReadOnlyCollection<Playlist> Playlists => _playlists.AsReadOnly();

    public string Email { get; private set; }
    public string UserName { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public ThemeColor ThemeColor { get; private set; }
    public bool IsAdmin { get; private set; }
    public string PasswordHash { get; private set; }
    public string EmailConfirmationToken { get; private set; }
    public string ForgotPasswordToken { get; private set; }
    public string RefreshToken { get; private set; }

    private User() { }

    public static User Create(string email, string userName, ThemeColor themeColor, bool isAdmin, bool emailConfirmed = false)
    {
        return new User
        {
            Id = 0,
            Email = email,
            UserName = userName,
            ThemeColor = themeColor,
            IsAdmin = isAdmin,
            EmailConfirmed = emailConfirmed,
        };
    }

    public void SetPassword(string plainTextPassword, IPasswordService passwordService)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            throw new MyValidationException("Password", nameof(plainTextPassword)); // TODO: custom exceptions

        PasswordHash = passwordService.Hash(plainTextPassword);
        UpdateModified();
    }

    public bool VerifyPassword(string password, IPasswordService passwordService)
    {
        return passwordService.Validate(password, PasswordHash);
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash)); // TODO: custom exceptions

        PasswordHash = passwordHash;
        UpdateModified();
    }

    public void SetEmailConfirmationToken(string token)
    {
        EmailConfirmationToken = token;
        UpdateModified();
    }
    
    public void SetForgotPasswordToken(string token)
    {
        ForgotPasswordToken = token;
        UpdateModified();
    }
    
    public void SetRefreshToken(string token)
    {
        RefreshToken = token;
        UpdateModified();
    }
    
    public void SetThemeColor(ThemeColor themeColor)
    {
        ThemeColor = themeColor;
        UpdateModified();
    }
    
    public void SetEmailConfirmed(bool emailConfirmed)
    {
        EmailConfirmed = emailConfirmed;
        UpdateModified();
    }

    public Playlist AddPlaylist(string name, bool isPublic)
    {
        var playlist = Playlist.Create(name, isPublic, this);
        _playlists.Add(playlist);
        UpdateModified();
        return playlist;
    }
}