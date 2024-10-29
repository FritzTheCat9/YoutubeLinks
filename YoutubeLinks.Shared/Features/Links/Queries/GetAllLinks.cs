using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Queries;

public static class GetAllLinks
{
    public class Query : IRequest<IEnumerable<LinkInfoDto>>
    {
        public int PlaylistId { get; init; }
        public bool Downloaded { get; init; }
    }

    public class Validator : AbstractValidator<Query> { }

    public class LinkInfoDto
    {
        public int Id { get; init; }
        public string Url { get; set; }
        public string VideoId { get; set; }
        public string Title { get; set; }
    }
}