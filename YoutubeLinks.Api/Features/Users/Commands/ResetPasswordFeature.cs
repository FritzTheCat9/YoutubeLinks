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

public static class ResetPasswordFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/resetPassword", async (
                ResetPassword.Command command,
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
        IPasswordService passwordService,
        IEmailService emailService,
        IStringLocalizer<ApiValidationMessage> validationLocalizer)
        : IRequestHandler<ResetPassword.Command, bool>
    {
        public async Task<bool> Handle(
            ResetPassword.Command command,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(ResetPassword.Command.Email),
                           validationLocalizer[
                               nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            var isTokenAssignedToUser =
                await userRepository.IsForgotPasswordTokenAssignedToUser(user.Email, command.Token);
            if (!isTokenAssignedToUser)
            {
                throw new MyValidationException(nameof(ResetPassword.Command.Token),
                    validationLocalizer[nameof(ApiValidationMessageString.TokenIsNotAssignedToThisUser)]);
            }

            if (passwordService.Validate(command.NewPassword, user.Password))
            {
                throw new MyValidationException(nameof(ResetPassword.Command.NewPassword),
                    validationLocalizer[nameof(ApiValidationMessageString.NewPasswordShouldNotBeEqualToOldPassword)]);
            }

            user.Password = passwordService.Hash(command.NewPassword);
            user.ForgotPasswordToken = null;

            await userRepository.Update(user);

            await emailService.SendEmail(user.Email, new ResetPasswordSuccessTemplateModel
            {
                UserName = user.UserName
            });

            return true;
        }
    }
}