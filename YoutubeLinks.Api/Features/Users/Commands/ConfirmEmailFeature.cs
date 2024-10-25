using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Api.Features.Users.Commands;

public static class ConfirmEmailFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/confirmEmail", async (
                ConfirmEmail.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }

    public class Handler(
        IUserRepository userRepository,
        IEmailService emailService,
        IStringLocalizer<ApiValidationMessage> validationLocalizer)
        : IRequestHandler<ConfirmEmail.Command, bool>
    {
        public async Task<bool> Handle(
            ConfirmEmail.Command command,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(ConfirmEmail.Command.Email),
                           validationLocalizer[
                               nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            if (user.EmailConfirmed)
            {
                throw new MyValidationException(nameof(ConfirmEmail.Command.Email),
                    validationLocalizer[nameof(ApiValidationMessageString.EmailAlreadyConfirmed)]);
            }

            var isTokenAssignedToUser =
                await userRepository.IsEmailConfirmationTokenAssignedToUser(user.Email, command.Token);
            if (!isTokenAssignedToUser)
            {
                throw new MyValidationException(nameof(ConfirmEmail.Command.Token),
                    validationLocalizer[nameof(ApiValidationMessageString.TokenIsNotAssignedToThisUser)]);
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;

            await userRepository.Update(user);

            await emailService.SendEmail(user.Email, new EmailConfirmationSuccessfulTemplateModel
            {
                UserName = user.UserName
            });

            return true;
        }
    }
}