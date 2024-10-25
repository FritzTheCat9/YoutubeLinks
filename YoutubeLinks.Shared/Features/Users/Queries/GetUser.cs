using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Features.Users.Queries;

public static class GetUser
{
    public class Query : IRequest<UserDto>
    {
        public int Id { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
    }
}