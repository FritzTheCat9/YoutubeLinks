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

    public class Handler(
        IUserRepository userRepository,
        IEmailService emailService,
        IEmailConfirmationService emailConfirmationService,
        IStringLocalizer<ApiValidationMessage> validationLocalizer)
        : IRequestHandler<ResendConfirmationEmail.Command, Unit>
    {
        public async Task<Unit> Handle(
            ResendConfirmationEmail.Command command,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(ResendConfirmationEmail.Command.Email),
                           validationLocalizer[
                               nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            if (user.EmailConfirmed)
            {
                throw new MyValidationException(nameof(ResendConfirmationEmail.Command.Email),
                    validationLocalizer[nameof(ApiValidationMessageString.EmailAlreadyConfirmed)]);
            }

            user.EmailConfirmationToken = emailConfirmationService.GenerateEmailConfirmationToken(command.Email);
            await userRepository.Update(user);

            await emailService.SendEmail(user.Email, new EmailConfirmationTemplateModel
            {
                UserName = user.UserName,
                Link = emailConfirmationService.GenerateConfirmationLink(user.Email, user.EmailConfirmationToken)
            });

            return Unit.Value;
        }
    }
}