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

    public class Handler(
        IUserRepository userRepository,
        IEmailService emailService,
        IForgotPasswordService forgotPasswordService,
        IStringLocalizer<ApiValidationMessage> validationLocalizer)
        : IRequestHandler<ForgotPassword.Command, Unit>
    {
        public async Task<Unit> Handle(
            ForgotPassword.Command command,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(ForgotPassword.Command.Email),
                           validationLocalizer[
                               nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            if (!user.EmailConfirmed)
                throw new MyValidationException(nameof(ForgotPassword.Command.Email),
                    validationLocalizer[nameof(ApiValidationMessageString.EmailIsNotConfirmed)]);

            user.ForgotPasswordToken = forgotPasswordService.GenerateForgotPasswordToken(command.Email);
            await userRepository.Update(user);

            await emailService.SendEmail(user.Email, new ForgotPasswordTemplateModel
            {
                UserName = user.UserName,
                Link = forgotPasswordService.GenerateForgotPasswordLink(user.Email, user.ForgotPasswordToken)
            });

            return Unit.Value;
        }
    }
}