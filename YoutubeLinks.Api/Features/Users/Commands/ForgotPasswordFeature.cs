using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Api.Features.Users.Commands
{
    public static class ForgotPasswordFeature
    {
        public static void Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users/forgotPassword", async (
                ForgotPassword.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) 
                    => Results.Ok(await mediator.Send(command, cancellationToken)))
                .WithTags(Tags.Users)
                .AllowAnonymous();
        }

        public class Handler : IRequestHandler<ForgotPassword.Command, Unit>
        {
            private readonly IUserRepository _userRepository;
            private readonly IEmailService _emailService;
            private readonly IForgotPasswordService _forgotPasswordService;
            private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer;

            public Handler(
                IUserRepository userRepository,
                IEmailService emailService,
                IForgotPasswordService forgotPasswordService,
                IStringLocalizer<ApiValidationMessage> validationLocalizer)
            {
                _userRepository = userRepository;
                _emailService = emailService;
                _forgotPasswordService = forgotPasswordService;
                _validationLocalizer = validationLocalizer;
            }

            public async Task<Unit> Handle(
                ForgotPassword.Command command,
                CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByEmail(command.Email) ??
                    throw new MyValidationException(nameof(ForgotPassword.Command.Email),
                        _validationLocalizer[nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

                if (!user.EmailConfirmed)
                    throw new MyValidationException(nameof(ForgotPassword.Command.Email),
                        _validationLocalizer[nameof(ApiValidationMessageString.EmailIsNotConfirmed)]);

                user.ForgotPasswordToken = _forgotPasswordService.GenerateForgotPasswordToken(command.Email);
                await _userRepository.Update(user);

                await _emailService.SendEmail(user.Email, new ForgotPasswordTemplateModel
                {
                    UserName = user.UserName,
                    Link = _forgotPasswordService.GenerateForgotPasswordLink(user.Email, user.ForgotPasswordToken),
                });

                return Unit.Value;
            }
        }
    }
}
