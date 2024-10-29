using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Shared.Features.Links.Queries;

public static class GetLink
{
    public class Query : IRequest<LinkDto>
    {
        public int Id { get; init; }
    }

    public class Validator : AbstractValidator<Query> { }
}