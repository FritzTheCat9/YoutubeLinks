using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Shared.Features.Links.Queries
{
    public class GetAllLinks
    {
        public class Query : IRequest<PagedList<LinkDto>>, IPagedQuery, ISortedQuery
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public string SortColumn { get; set; }
            public SortOrder SortOrder { get; set; }
            public string SearchTerm { get; set; }

            public int PlaylistId { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {

        }
    }
}
