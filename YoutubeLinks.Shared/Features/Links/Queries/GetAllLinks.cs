using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Queries
{
    public static class GetAllLinks
    {
        public class Query : IRequest<IEnumerable<LinkInfoDto>>
        {
            public int PlaylistId { get; set; }
            public bool Downloaded { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {

        }

        public class LinkInfoDto
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public string VideoId { get; set; }
            public string Title { get; set; }
        }
    }
}
