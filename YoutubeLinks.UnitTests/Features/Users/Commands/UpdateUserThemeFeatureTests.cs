﻿using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.UnitTests.Features.Users.Commands
{
    public class UpdateUserThemeFeatureTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;

        public UpdateUserThemeFeatureTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _clock = Substitute.For<IClock>();
        }

        [Fact]
        public async Task UpdateUserThemeHandler_ThrowsForbiddenException_IfCommandUserIdIsNotEqualToCurrentLoggedUserId()
        {
            var command = new UpdateUserTheme.Command
            {
                Id = 1,
                ThemeColor = ThemeColor.Light,
            };

            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            mediator.Send(Arg.Any<UpdateUserTheme.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new UpdateUserThemeFeature.Handler(_userRepository, authService, _clock);
                    return handler.Handle(callInfo.Arg<UpdateUserTheme.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
        }

        [Fact]
        public async Task UpdateUserThemeHandler_ThrowsNotFoundException_IfUserIsNotFound()
        {
            var command = new UpdateUserTheme.Command
            {
                Id = 1,
                ThemeColor = ThemeColor.Light,
            };

            var authService = Substitute.For<IAuthService>();
            var userRepository = Substitute.For<IUserRepository>();
            var mediator = Substitute.For<IMediator>();

            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
            userRepository.Get(Arg.Any<int>()).Returns((User)null);

            mediator.Send(Arg.Any<UpdateUserTheme.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new UpdateUserThemeFeature.Handler(userRepository, authService, _clock);
                    return handler.Handle(callInfo.Arg<UpdateUserTheme.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
        }

        [Fact]
        public async Task UpdateUserThemeHandler_UpdatesUserTheme()
        {
            var command = new UpdateUserTheme.Command
            {
                Id = 1,
                ThemeColor = ThemeColor.Light,
            };

            var authService = Substitute.For<IAuthService>();
            var userRepository = Substitute.For<IUserRepository>();
            var mediator = Substitute.For<IMediator>();

            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
            userRepository.Get(Arg.Any<int>()).Returns(new User());

            mediator.Send(Arg.Any<UpdateUserTheme.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new UpdateUserThemeFeature.Handler(userRepository, authService, _clock);
                    return handler.Handle(callInfo.Arg<UpdateUserTheme.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            await userRepository.Received().Update(Arg.Any<User>());
        }
    }
}
