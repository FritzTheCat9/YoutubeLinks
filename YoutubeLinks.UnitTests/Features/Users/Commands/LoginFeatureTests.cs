using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api;
using NSubstitute;
using MediatR;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using FluentAssertions;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.UnitTests.Features.Users.Commands
{
    public class LoginFeatureTests
    {
        private readonly IPasswordService _passwordService;
        private readonly IAuthenticator _authenticator;
        private readonly IStringLocalizer<ApiValidationMessage> _localizer;

        public LoginFeatureTests()
        {
            _passwordService = Substitute.For<IPasswordService>();
            _authenticator = Substitute.For<IAuthenticator>();
            _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
        }

        [Fact]
        public async Task LoginHandler_ThrowsValidationException_IfUserWithGivenEmailDoesNotExist()
        {
            var command = new Login.Command
            {
                Email = "test@test.com",
                Password = "password",
            };

            var userRepository = Substitute.For<IUserRepository>();
            var mediator = Substitute.For<IMediator>();

            userRepository.GetByEmail(Arg.Any<string>()).Returns(Task.FromResult<User>(null));

            mediator.Send(Arg.Any<Login.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new LoginFeature.Handler(_passwordService, userRepository, _authenticator, _localizer);
                    return handler.Handle(callInfo.Arg<Login.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyValidationException>(action);
        }

        [Fact]
        public async Task LoginHandler_ThrowsValidationException_IfUserEmailIsNotConfirmed()
        {
            var command = new Login.Command
            {
                Email = "test@test.com",
                Password = "password",
            };

            var userRepository = Substitute.For<IUserRepository>();
            var mediator = Substitute.For<IMediator>();

            userRepository.GetByEmail(Arg.Any<string>()).Returns(new User
            {
                EmailConfirmed = false,
            });

            mediator.Send(Arg.Any<Login.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new LoginFeature.Handler(_passwordService, userRepository, _authenticator, _localizer);
                    return handler.Handle(callInfo.Arg<Login.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyValidationException>(action);
        }

        [Fact]
        public async Task LoginHandler_ThrowsValidationException_IfPasswordIsIncorrect()
        {
            var command = new Login.Command
            {
                Email = "test@test.com",
                Password = "password",
            };

            var userRepository = Substitute.For<IUserRepository>();
            var passwordService = Substitute.For<IPasswordService>();
            var mediator = Substitute.For<IMediator>();

            userRepository.GetByEmail(Arg.Any<string>()).Returns(new User
            {
                EmailConfirmed = true,
            });
            passwordService.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            mediator.Send(Arg.Any<Login.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new LoginFeature.Handler(passwordService, userRepository, _authenticator, _localizer);
                    return handler.Handle(callInfo.Arg<Login.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyValidationException>(action);
        }

        [Fact]
        public async Task LoginHandler_ReturnsLoginJwtToken()
        {
            var command = new Login.Command
            {
                Email = "test@test.com",
                Password = "password",
            };

            var userRepository = Substitute.For<IUserRepository>();
            var passwordService = Substitute.For<IPasswordService>();
            var authenticator = Substitute.For<IAuthenticator>();
            var mediator = Substitute.For<IMediator>();

            userRepository.GetByEmail(Arg.Any<string>()).Returns(new User
            {
                EmailConfirmed = true,
            });
            passwordService.Validate(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            authenticator.CreateTokens(Arg.Any<User>()).Returns(new JwtDto
            {
                 AccessToken = "AccessToken"
            });

            mediator.Send(Arg.Any<Login.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new LoginFeature.Handler(passwordService, userRepository, authenticator, _localizer);
                    return handler.Handle(callInfo.Arg<Login.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeOfType<JwtDto>();
            authenticator.Received().CreateTokens(Arg.Any<User>());
        }
    }
}
