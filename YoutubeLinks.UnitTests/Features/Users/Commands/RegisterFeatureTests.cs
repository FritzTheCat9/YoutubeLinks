using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.UnitTests.Features.Users.Commands;

public class RegisterFeatureTests
{
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    [Fact]
    public async Task RegisterHandler_ThrowsValidationException_IfEmailExists()
    {
        var command = new Register.Command
        {
            Email = "test@test.com",
            UserName = "Test",
            Password = "password",
            RepeatPassword = "password"
        };

        _userRepository.EmailExists(Arg.Any<string>()).Returns(true);

        var handler = new RegisterFeature.Handler(_passwordService, _userRepository, _emailService,
            _tokenService, _validationLocalizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task RegisterHandler_ThrowsValidationException_IfUserNameExists()
    {
        var command = new Register.Command
        {
            Email = "test@test.com",
            UserName = "Test",
            Password = "password",
            RepeatPassword = "password"
        };

        _userRepository.EmailExists(Arg.Any<string>()).Returns(false);
        _userRepository.UserNameExists(Arg.Any<string>()).Returns(true);

        var handler = new RegisterFeature.Handler(_passwordService, _userRepository, _emailService,
            _tokenService, _validationLocalizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task RegisterHandler_RegistersUser()
    {
        var command = new Register.Command
        {
            Email = "test@test.com",
            UserName = "Test",
            Password = "password",
            RepeatPassword = "password"
        };

        _userRepository.EmailExists(Arg.Any<string>()).Returns(false);
        _userRepository.UserNameExists(Arg.Any<string>()).Returns(false);
        _userRepository.Create(Arg.Any<User>()).Returns(1);

        var handler = new RegisterFeature.Handler(_passwordService, _userRepository, _emailService,
                    _tokenService, _validationLocalizer);
        var result = await handler.Handle(command, CancellationToken.None);

        await _userRepository.Received().Create(Arg.Any<User>());
        await _emailService.Received().SendEmail(Arg.Any<string>(), Arg.Any<EmailConfirmationTemplateModel>());
        _tokenService.Received().GenerateLink(Arg.Any<string>(), Arg.Any<string>(), LinkType.ConfirmEmail);
        Assert.Equal(1, result);
    }
}