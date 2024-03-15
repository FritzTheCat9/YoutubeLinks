using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Api.Features.Links.Queries
{
    public static class GetLinkFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/links/{id}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLink.Query() { Id = id };
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
                .WithName("GetLink")
                .WithTags("Links")
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<GetLink.Query, LinkDto>
        {
            private readonly ILinkRepository _linkRepository;
            private readonly IAuthService _authService;

            public Handler(
                ILinkRepository linkRepository,
                IAuthService authService)
            {
                _linkRepository = linkRepository;
                _authService = authService;
            }

            public async Task<LinkDto> Handle(
                GetLink.Query query,
                CancellationToken cancellationToken)
            {
                var link = await _linkRepository.Get(query.Id) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(link.Playlist.UserId);
                if (!link.Playlist.Public
                    && !isUserPlaylist)
                    throw new MyForbiddenException();

                return link.ToDto();
            }
        }
    }
}
