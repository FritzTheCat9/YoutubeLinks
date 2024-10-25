using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Features.Users.Queries;

public static class GetAllUsers
{
    public class Query : IRequest<PagedList<UserDto>>, IPagedQuery, ISortedQuery
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public SortOrder SortOrder { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
    }
}