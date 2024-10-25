using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Abstractions;
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
    private readonly IClock _clock = Substitute.For<IClock>();
    private readonly IEmailConfirmationService _emailConfirmationService = Substitute.For<IEmailConfirmationService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();

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

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.EmailExists(Arg.Any<string>()).Returns(true);

        mediator.Send(Arg.Any<Register.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new RegisterFeature.Handler(_clock, _passwordService, userRepository, _emailService,
                    _emailConfirmationService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<Register.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
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

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.EmailExists(Arg.Any<string>()).Returns(false);
        userRepository.UserNameExists(Arg.Any<string>()).Returns(true);

        mediator.Send(Arg.Any<Register.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new RegisterFeature.Handler(_clock, _passwordService, userRepository, _emailService,
                    _emailConfirmationService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<Register.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
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

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.EmailExists(Arg.Any<string>()).Returns(false);
        userRepository.UserNameExists(Arg.Any<string>()).Returns(false);
        userRepository.Create(Arg.Any<User>()).Returns(1);

        mediator.Send(Arg.Any<Register.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new RegisterFeature.Handler(_clock, _passwordService, userRepository, _emailService,
                    _emailConfirmationService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<Register.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        await userRepository.Received().Create(Arg.Any<User>());
        await _emailService.Received().SendEmail(Arg.Any<string>(), Arg.Any<EmailConfirmationTemplateModel>());
        _emailConfirmationService.Received().GenerateConfirmationLink(Arg.Any<string>(), Arg.Any<string>());
        result.Should().Be(1);
    }
}