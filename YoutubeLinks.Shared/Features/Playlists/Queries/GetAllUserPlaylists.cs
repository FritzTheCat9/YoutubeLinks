using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Shared.Features.Playlists.Queries;

public static class GetAllUserPlaylists
{
    public class Query : IRequest<PagedList<PlaylistDto>>, IPagedQuery, ISortedQuery
    {
        public string SearchTerm { get; set; }
        public int UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public SortOrder SortOrder { get; set; }
    }

    public class Validator : AbstractValidator<Query> { }
}