using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Users.Commands;

public static class UpdateUserThemeFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{id:int}/theme", async (
                int id,
                UpdateUserTheme.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
            .WithTags(Tags.Users)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IUserRepository userRepository,
        IAuthService authService)
        : IRequestHandler<UpdateUserTheme.Command, Unit>
    {
        public async Task<Unit> Handle(
            UpdateUserTheme.Command command,
            CancellationToken cancellationToken)
        {
            var isLoggedInUser = authService.IsLoggedInUser(command.Id);
            if (!isLoggedInUser)
            {
                throw new MyForbiddenException();
            }

            var user = await userRepository.Get(command.Id) ?? throw new MyNotFoundException();

            user.SetThemeColor(command.ThemeColor);

            await userRepository.Update(user);
            return Unit.Value;
        }
    }
}