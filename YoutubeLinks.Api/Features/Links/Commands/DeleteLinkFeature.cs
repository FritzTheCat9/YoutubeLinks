﻿using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands
{
    public static class DeleteLinkFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/links/{id}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteLink.Command() { Id = id };
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
                .WithTags(Tags.Links)
                .RequireAuthorization(Policy.User);

            return app;
        }

        public class Handler : IRequestHandler<DeleteLink.Command, Unit>
        {
            private readonly IAuthService _authService;
            private readonly ILinkRepository _linkRepository;

            public Handler(
                IAuthService authService,
                ILinkRepository linkRepository)
            {
                _authService = authService;
                _linkRepository = linkRepository;
            }

            public async Task<Unit> Handle(
                DeleteLink.Command command,
                CancellationToken cancellationToken)
            {
                var link = await _linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(link.Playlist.UserId);
                if (!isUserPlaylist)
                    throw new MyForbiddenException();

                await _linkRepository.Delete(link);
                return Unit.Value;
            }
        }
    }
}
