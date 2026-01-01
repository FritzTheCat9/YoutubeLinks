using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Builders;

public class UserBuilder
{
    private string _email = "test@mail.com";
    private string _name = "Test User";
    private ThemeColor _theme = ThemeColor.Dark;
    private bool _allowNsfw = true;
    private bool _allowHistory = true;

    public UserBuilder WithEmail(string email) { _email = email; return this; }
    public UserBuilder WithName(string name) { _name = name; return this; }
    public UserBuilder WithTheme(ThemeColor theme) { _theme = theme; return this; }

    public User Build()
        => User.Create(_email, _name, _theme, _allowNsfw, _allowHistory);

    public static UserBuilder Create() => new();
}