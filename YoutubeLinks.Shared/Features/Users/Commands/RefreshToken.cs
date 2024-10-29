using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Features.Users.Commands;

public static class RefreshToken
{
    public class Command : IRequest<JwtDto>
    {
        public string RefreshToken { get; init; }
    }

    public class Validator : AbstractValidator<Command> { }
}