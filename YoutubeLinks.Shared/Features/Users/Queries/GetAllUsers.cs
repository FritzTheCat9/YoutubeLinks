using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Features.Users.Queries;

public static class GetAllUsers
{
    public class Query : QueryParameters, IRequest<PagedList<UserDto>> { }

    public class Validator : AbstractValidator<Query> { }
}