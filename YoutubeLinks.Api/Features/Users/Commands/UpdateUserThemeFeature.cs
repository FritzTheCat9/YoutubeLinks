using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Users.Commands
{
    public static class UpdateUserThemeFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/users/{id}/theme", async (
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

            return app;
        }

        public class Handler : IRequestHandler<UpdateUserTheme.Command, Unit>
        {
            private readonly IUserRepository _userRepository;
            private readonly IAuthService _authService;
            private readonly IClock _clock;

            public Handler(
                IUserRepository userRepository,
                IAuthService authService,
                IClock clock)
            {
                _userRepository = userRepository;
                _authService = authService;
                _clock = clock;
            }

            public async Task<Unit> Handle(
                UpdateUserTheme.Command command,
                CancellationToken cancellationToken)
            {
                var isLoggedInUser = _authService.IsLoggedInUser(command.Id);
                if (!isLoggedInUser)
                    throw new MyForbiddenException();

                var user = await _userRepository.Get(command.Id) ?? throw new MyNotFoundException();

                user.ThemeColor = command.ThemeColor;
                user.Modified = _clock.Current();

                await _userRepository.Update(user);
                return Unit.Value;
            }
        }
    }
}
