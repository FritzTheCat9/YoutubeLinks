using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Api.Features.Links.Queries;

public static class GetLinkFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/links/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLink.Query { Id = id };
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
            .WithName("GetLink")
            .WithTags(Tags.Links)
            .AllowAnonymous();
    }

    public class Handler(
        ILinkRepository linkRepository,
        IAuthService authService)
        : IRequestHandler<GetLink.Query, LinkDto>
    {
        public async Task<LinkDto> Handle(
            GetLink.Query query,
            CancellationToken cancellationToken)
        {
            var link = await linkRepository.Get(query.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(link.Playlist.UserId);
            if (!link.Playlist.Public
                && !isUserPlaylist)
                throw new MyForbiddenException();

            return link.ToDto();
        }
    }
}