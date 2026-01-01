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

    private readonly IStringLocalizer<ApiValidationMessage> _localizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();

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
            Password = "password"
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
        _authenticator.CreateTokens(Arg.Any<User>()).Returns(new JwtDto
        {
            AccessToken = "AccessToken"
        });

        var handler = new LoginFeature.Handler(_passwordService, _userRepository, _authenticator, _localizer);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<JwtDto>(result);
        _authenticator.Received().CreateTokens(Arg.Any<User>());
    }
}