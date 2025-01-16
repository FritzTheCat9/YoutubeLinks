using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Shared.Features.Links.Queries;

public static class GetAllPaginatedLinks
{
    public class Query : QueryParameters, IRequest<PagedList<LinkDto>>
    {
        public int PlaylistId { get; set; }
    }

    public class Validator : AbstractValidator<Query> { }
}