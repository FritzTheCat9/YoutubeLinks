using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Emails.Models;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Api.Features.Users.Commands;

public static class RegisterFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/register", async (
                Register.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = await mediator.Send(command, cancellationToken);
                return Results.CreatedAtRoute("GetUser", new { id = userId }, userId);
            })
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }

    public class Handler(
        IPasswordService passwordService,
        IUserRepository userRepository,
        IEmailService emailService,
        ITokenService tokenService,
        IStringLocalizer<ApiValidationMessage> validationLocalizer)
        : IRequestHandler<Register.Command, int>
    {
        public async Task<int> Handle(
            Register.Command command,
            CancellationToken cancellationToken)
        {
            await ValidateCommand(command);

            var user = User.Create(command.Email, command.UserName, command.ThemeColor, false);
            user.SetPassword(command.Password, passwordService);
            user.SetEmailConfirmationToken(tokenService.GenerateToken(command.Email));
            
            var userId = await userRepository.Create(user);

            await emailService.SendEmail(user.Email, new EmailConfirmationTemplateModel
            {
                UserName = command.UserName,
                Link = tokenService.GenerateLink(user.Email, user.EmailConfirmationToken, LinkType.ConfirmEmail)
            });

            return userId;
        }

        private async Task ValidateCommand(Register.Command command)
        {
            var emailExists = await userRepository.EmailExists(command.Email);
            if (emailExists)
            {
                throw new MyValidationException(nameof(Register.Command.Email),
                    validationLocalizer[nameof(ApiValidationMessageString.EmailIsAlreadyTaken)]);
            }

            var userNameExists = await userRepository.UserNameExists(command.UserName);
            if (userNameExists)
            {
                throw new MyValidationException(nameof(Register.Command.UserName),
                    validationLocalizer[nameof(ApiValidationMessageString.UserNameIsAlreadyTaken)]);
            }
        }
    }
}