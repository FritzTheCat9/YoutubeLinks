using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Api.Features.Users.Commands;

public static class LoginFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/login", async (
                    Login.Command command,
                    IMediator mediator,
                    CancellationToken cancellationToken)
                => Results.Ok(await mediator.Send(command, cancellationToken)))
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }

    public class Handler(
        IPasswordService passwordService,
        IUserRepository userRepository,
        IAuthenticator authenticator,
        IStringLocalizer<ApiValidationMessage> localizer)
        : IRequestHandler<Login.Command, JwtDto>
    {
        public async Task<JwtDto> Handle(Login.Command command, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmail(command.Email) ??
                       throw new MyValidationException(nameof(Login.Command.Email),
                           localizer[nameof(ApiValidationMessageString.EmailUserWithGivenEmailDoesNotExist)]);

            if (!user.EmailConfirmed)
            {
                throw new MyValidationException(nameof(Login.Command.Email),
                    localizer[nameof(ApiValidationMessageString.EmailIsNotConfirmed)]);
            }

            if (!passwordService.Validate(command.Password, user.Password))
            {
                throw new MyValidationException(nameof(Login.Command.Password),
                    localizer[nameof(ApiValidationMessageString.PasswordIsIncorrect)]);
            }

            var jwt = authenticator.CreateTokens(user);

            user.RefreshToken = jwt.RefreshToken;
            await userRepository.Update(user);

            return jwt;
        }
    }
}