using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Commands;

public static class DeleteLink
{
    public class Command : IRequest<Unit>
    {
        public int Id { get; init; }
    }

    public class Validator : AbstractValidator<Command> { }
}