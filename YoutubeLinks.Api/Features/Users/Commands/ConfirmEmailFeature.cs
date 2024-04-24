using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Emails.Models;

namespace YoutubeLinks.Api.Features.Users.Commands
{
    public static class ConfirmEmailFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users/confirmEmail", async (ConfirmEmail.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
                .WithTags(Tags.Users)
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<ConfirmEmail.Command, bool>
        {
            private readonly IUserRepository _userRepository;
            private readonly IEmailService _emailService;
            private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer;

            public Handler(
                IUserRepository userRepository,
                IEmailService emailService,
                IStringLocalizer<ApiValidationMessage> validationLocalizer)
            {
                _userRepository = userRepository;
                _emailService = emailService;
                _validationLocalizer = validationLocalizer;
            }

            public async Task<bool> Handle(
                ConfirmEmail.Command command,
                CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByEmail(command.Email) ??
                    throw new MyValidationException(nameof(ConfirmEmail.Command.Email),
                        _validationLocalizer[nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

                if (user.EmailConfirmed)
                    throw new MyValidationException(nameof(ConfirmEmail.Command.Email),
                        _validationLocalizer[nameof(ApiValidationMessageString.EmailAlreadyConfirmed)]);

                var isTokenAssignedToUser = await _userRepository.IsTokenAssignedToUser(user.Email, command.Token);
                if (!isTokenAssignedToUser)
                    throw new MyValidationException(nameof(ConfirmEmail.Command.Token),
                        _validationLocalizer[nameof(ApiValidationMessageString.TokenIsNotAssignedToThisUser)]);

                user.EmailConfirmed = true;
                user.EmailConfirmationToken = null;

                await _userRepository.Update(user);

                await _emailService.SendEmail(user.Email, new EmailConfirmationSuccessfulTemplateModel
                {
                    UserName = user.UserName,
                });

                return isTokenAssignedToUser;
            }
        }
    }
}
