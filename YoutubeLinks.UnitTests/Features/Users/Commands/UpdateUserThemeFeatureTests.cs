using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Users.Commands;

public class UpdateUserThemeFeatureTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    [Fact]
    public async Task UpdateUserThemeHandler_ThrowsForbiddenException_IfCommandUserIdIsNotEqualToCurrentLoggedUserId()
    {
        var command = new UpdateUserTheme.Command
        {
            Id = 1,
            ThemeColor = ThemeColor.Light
        };

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        var handler = new UpdateUserThemeFeature.Handler(_userRepository, _authService);

        await Assert.ThrowsAsync<MyForbiddenException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateUserThemeHandler_ThrowsNotFoundException_IfUserIsNotFound()
    {
        var command = new UpdateUserTheme.Command
        {
            Id = 1,
            ThemeColor = ThemeColor.Light
        };

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _userRepository.Get(Arg.Any<int>()).Returns((User)null);

        var handler = new UpdateUserThemeFeature.Handler(_userRepository, _authService);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateUserThemeHandler_UpdatesUserTheme()
    {
        var command = new UpdateUserTheme.Command
        {
            Id = 1,
            ThemeColor = ThemeColor.Light
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);

        _authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        _userRepository.Get(Arg.Any<int>()).Returns(user);

        var handler = new UpdateUserThemeFeature.Handler(_userRepository, _authService);
        await handler.Handle(command, CancellationToken.None);

        await _userRepository.Received().Update(Arg.Any<User>());
    }
}