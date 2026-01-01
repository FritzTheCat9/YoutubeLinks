using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Users.Commands;

public class ConfirmEmailFeatureTests
{
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer =
        Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfUserWithGivenEmailDoesNotExist()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(Task.FromResult<User>(null));

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _validationLocalizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfEmailIsAlreadyConfirmed()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _validationLocalizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfTokenIsNotAssignedToUser()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, false);

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);
        _userRepository.IsEmailConfirmationTokenAssignedToUser(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _validationLocalizer);

        await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailHandler_ConfirmsEmail()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, false);

        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);
        _userRepository.IsEmailConfirmationTokenAssignedToUser(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _validationLocalizer);
        var result = await handler.Handle(command, CancellationToken.None);

        await _userRepository.Received().Update(Arg.Any<User>());
        await _emailService.Received()
            .SendEmail(Arg.Any<string>(), Arg.Any<EmailConfirmationSuccessfulTemplateModel>());
        Assert.True(result);
    }
}