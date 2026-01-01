using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.UnitTests.Features.Users.Commands;

public class LoginFeatureTests
{
    private readonly IAuthenticator _authenticator = Substitute.For<IAuthenticator>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    [Fact]
    public async Task LoginHandler_ThrowsValidationException_IfUserWithGivenEmailDoesNotExist()
    {
        var command = new Login.Command
        {
            Email = "test@test.com",
            Password = "password"
        };

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(Task.FromResult<User>(null));

        var handler = new LoginFeature.Handler(_passwordService, _userRepository, _authenticator, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task LoginHandler_ThrowsValidationException_IfUserEmailIsNotConfirmed()
    {
        var command = new Login.Command
        {
            Email = "test@test.com",
            Password = "password"
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, false);

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);

        var handler = new LoginFeature.Handler(_passwordService, _userRepository, _authenticator, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task LoginHandler_ThrowsValidationException_IfPasswordIsIncorrect()
    {
        var command = new Login.Command
        {
            Email = "test@test.com",
            Password = "wrongpassword"
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);
        _passwordService.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var handler = new LoginFeature.Handler(_passwordService, _userRepository, _authenticator, _localizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task LoginHandler_ReturnsLoginJwtToken()
    {
        var command = new Login.Command
        {
            Email = "test@test.com",
            Password = "password"
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);
        _passwordService.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        var jwtDto = new JwtDto
        {
            AccessToken = "AccessToken",
            RefreshToken = "RefreshToken"
        };
        _authenticator.CreateTokens(Arg.Any<User>()).Returns(jwtDto);

        var handler = new LoginFeature.Handler(_passwordService, _userRepository, _authenticator, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<JwtDto>(result);
        Assert.Equal("AccessToken", result.AccessToken);
        Assert.Equal("RefreshToken", result.RefreshToken);

        _authenticator.Received().CreateTokens(Arg.Any<User>());
        await _userRepository.Received().Update(Arg.Any<User>());
    }
}
