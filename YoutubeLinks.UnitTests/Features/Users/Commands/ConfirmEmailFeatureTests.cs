using FluentAssertions;
using MediatR;
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

namespace YoutubeLinks.UnitTests.Features.Users.Commands;

public class ConfirmEmailFeatureTests
{
    private readonly IEmailService _emailService;
    private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer;

    public ConfirmEmailFeatureTests()
    {
        _emailService = Substitute.For<IEmailService>();
        _validationLocalizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfUserWithGivenEmailDoesNotExist()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.GetByEmail(Arg.Any<string>()).Returns(Task.FromResult<User>(null));

        mediator.Send(Arg.Any<ConfirmEmail.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ConfirmEmailFeature.Handler(userRepository, _emailService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<ConfirmEmail.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfEmailIsAlreadyConfirmed()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.GetByEmail(Arg.Any<string>()).Returns(new User
        {
            EmailConfirmed = true
        });

        mediator.Send(Arg.Any<ConfirmEmail.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ConfirmEmailFeature.Handler(userRepository, _emailService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<ConfirmEmail.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
    }

    [Fact]
    public async Task ConfirmEmailHandler_ThrowsValidationException_IfTokenIsNotAssignedToUser()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.GetByEmail(Arg.Any<string>()).Returns(new User
        {
            EmailConfirmed = false
        });
        userRepository.IsEmailConfirmationTokenAssignedToUser(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        mediator.Send(Arg.Any<ConfirmEmail.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ConfirmEmailFeature.Handler(userRepository, _emailService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<ConfirmEmail.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
    }

    [Fact]
    public async Task ConfirmEmailHandler_ConfirmsEmail()
    {
        var command = new ConfirmEmail.Command
        {
            Email = "test@test.com",
            Token = "token"
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.GetByEmail(Arg.Any<string>()).Returns(new User
        {
            EmailConfirmed = false
        });
        userRepository.IsEmailConfirmationTokenAssignedToUser(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        mediator.Send(Arg.Any<ConfirmEmail.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new ConfirmEmailFeature.Handler(userRepository, _emailService, _validationLocalizer);
                return handler.Handle(callInfo.Arg<ConfirmEmail.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        await userRepository.Received().Update(Arg.Any<User>());
        await _emailService.Received()
            .SendEmail(Arg.Any<string>(), Arg.Any<EmailConfirmationSuccessfulTemplateModel>());
        result.Should().BeTrue();
    }
}