﻿using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Extensions;
using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Api.Features.Links.Queries
{
    public static class GetAllPaginatedLinksFeature
    {
        public static void Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/links/allPaginated", async (
                GetAllPaginatedLinks.Query query,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
                .WithTags(Tags.Links)
                .AllowAnonymous();
        }

        public class Handler : IRequestHandler<GetAllPaginatedLinks.Query, PagedList<LinkDto>>
        {
            private readonly ILinkRepository _linkRepository;
            private readonly IPlaylistRepository _playlistRepository;
            private readonly IAuthService _authService;

            public Handler(
                ILinkRepository linkRepository,
                IPlaylistRepository playlistRepository,
                IAuthService authService)
            {
                _linkRepository = linkRepository;
                _playlistRepository = playlistRepository;
                _authService = authService;
            }

            public async Task<PagedList<LinkDto>> Handle(
                GetAllPaginatedLinks.Query query,
                CancellationToken cancellationToken)
            {
                var playlist = await _playlistRepository.Get(query.PlaylistId) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                var linkQuery = _linkRepository.AsQueryable(query.PlaylistId, isUserPlaylist);

                linkQuery = linkQuery.FilterLinks(query);
                linkQuery = linkQuery.SortLinks(query);

                var linksPagedList = PageListExtensions<LinkDto>.Create(linkQuery.Select(x => x.ToDto()),
                                                                                  query.Page,
                                                                                  query.PageSize);

                return linksPagedList;
            }
        }
    }
}
