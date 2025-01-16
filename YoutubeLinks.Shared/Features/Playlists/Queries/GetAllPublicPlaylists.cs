using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Shared.Features.Playlists.Queries;

public static class GetAllPublicPlaylists
{
    public class Query : QueryParameters, IRequest<PagedList<PlaylistDto>> { }

    public class Validator : AbstractValidator<Query> { }
}