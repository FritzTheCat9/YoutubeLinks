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
                    CancellationToken cancellationToken)
                => Results.Ok(await mediator.Send(command, cancellationToken)))
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }

    public class Handler : IRequestHandler<ResetPassword.Command, bool>
    {
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<ApiValidationMessage> _validationLocalizer;

        public Handler(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IEmailService emailService,
            IStringLocalizer<ApiValidationMessage> validationLocalizer)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            _validationLocalizer = validationLocalizer;
        }

        public async Task<bool> Handle(
            ResetPassword.Command command,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(ResetPassword.Command.Email),
                           _validationLocalizer[
                               nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            var isTokenAssignedToUser =
                await _userRepository.IsForgotPasswordTokenAssignedToUser(user.Email, command.Token);
            if (!isTokenAssignedToUser)
                throw new MyValidationException(nameof(ResetPassword.Command.Token),
                    _validationLocalizer[nameof(ApiValidationMessageString.TokenIsNotAssignedToThisUser)]);

            if (_passwordService.Validate(command.NewPassword, user.Password))
                throw new MyValidationException(nameof(ResetPassword.Command.NewPassword),
                    _validationLocalizer[nameof(ApiValidationMessageString.NewPasswordShouldNotBeEqualToOldPassword)]);

            user.Password = _passwordService.Hash(command.NewPassword);
            user.ForgotPasswordToken = null;

            await _userRepository.Update(user);

            await _emailService.SendEmail(user.Email, new ResetPasswordSuccessTemplateModel
            {
                UserName = user.UserName
            });

            return true;
        }
    }
}