using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.UnitTests.Features.Users.Commands;

public class ConfirmEmailFeatureTests
{
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();

    public ConfirmEmailFeatureTests()
    {
        _localizer[nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]
            .Returns(new LocalizedString(
                nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist),
                "User with this email does not exist."
            ));

        _localizer[nameof(ApiValidationMessageString.EmailAlreadyConfirmed)]
            .Returns(new LocalizedString(
                nameof(ApiValidationMessageString.EmailAlreadyConfirmed),
                "Email is already confirmed."
            ));

        _localizer[nameof(ApiValidationMessageString.TokenIsNotAssignedToThisUser)]
            .Returns(new LocalizedString(
                nameof(ApiValidationMessageString.TokenIsNotAssignedToThisUser),
                "Token is not assigned to this user."
            ));
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfUserDoesNotExist()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };
        _userRepository.GetByEmail(Arg.Any<string>()).Returns(Task.FromResult<User>(null));

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _localizer);

        var ex = await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("User with this email does not exist.", ex.Message);
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfEmailAlreadyConfirmed()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };
        var user = User.Create("user@test.com", "TestUser", ThemeColor.Light, true, true);
        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _localizer);

        var ex = await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("Email is already confirmed.", ex.Message);
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfTokenNotAssignedToUser()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };
        var user = User.Create("user@test.com", "TestUser", ThemeColor.Light, true, false);
        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);
        _userRepository.IsEmailConfirmationTokenAssignedToUser(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _localizer);

        var ex = await Assert.ThrowsAsync<MyValidationException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("Token is not assigned to this user.", ex.Message);
    }

    [Fact]
    public async Task ConfirmEmailHandler_ConfirmsEmailSuccessfully()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };
        var user = User.Create("user@test.com", "TestUser", ThemeColor.Light, true, false);
        _userRepository.GetByEmail(Arg.Any<string>()).Returns(user);
        _userRepository.IsEmailConfirmationTokenAssignedToUser(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        var handler = new ConfirmEmailFeature.Handler(_userRepository, _emailService, _localizer);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result, "Handler should return true on successful confirmation");
        Assert.True(user.EmailConfirmed, "User aggregate should be marked as confirmed");
        Assert.Null(user.EmailConfirmationToken);

        await _userRepository.Received(1).Update(Arg.Is<User>(u => u.EmailConfirmed && u.EmailConfirmationToken == null));
        await _emailService.Received(1)
            .SendEmail(user.Email, Arg.Is<EmailConfirmationSuccessfulTemplateModel>(m => m.UserName == user.UserName));
    }
}
