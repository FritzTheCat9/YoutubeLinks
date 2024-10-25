using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Api.Features.Users.Commands;

public static class ResendConfirmationEmailFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/resendConfirmationEmail", async (
                    ResendConfirmationEmail.Command command,
                    IMediator mediator,
                    CancellationToken cancellationToken)
                => Results.Ok(await mediator.Send(command, cancellationToken)))
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }

    public class Handler : IRequestHandler<ResendConfirmationEmail.Command, Unit>
    {
        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer;

        public Handler(
            IUserRepository userRepository,
            IEmailService emailService,
            IEmailConfirmationService emailConfirmationService,
            IStringLocalizer<ApiValidationMessage> validationLocalizer)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _emailConfirmationService = emailConfirmationService;
            _validationLocalizer = validationLocalizer;
        }

        public async Task<Unit> Handle(
            ResendConfirmationEmail.Command command,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(ResendConfirmationEmail.Command.Email),
                           _validationLocalizer[
                               nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            if (user.EmailConfirmed)
                throw new MyValidationException(nameof(ResendConfirmationEmail.Command.Email),
                    _validationLocalizer[nameof(ApiValidationMessageString.EmailAlreadyConfirmed)]);

            user.EmailConfirmationToken = _emailConfirmationService.GenerateEmailConfirmationToken(command.Email);
            await _userRepository.Update(user);

            await _emailService.SendEmail(user.Email, new EmailConfirmationTemplateModel
            {
                UserName = user.UserName,
                Link = _emailConfirmationService.GenerateConfirmationLink(user.Email, user.EmailConfirmationToken)
            });

            return Unit.Value;
        }
    }
}