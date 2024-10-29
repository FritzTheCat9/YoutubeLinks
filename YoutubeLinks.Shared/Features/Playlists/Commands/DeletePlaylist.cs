using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Playlists.Commands;

public static class DeletePlaylist
{
    public class Command : IRequest<Unit>
    {
        public int Id { get; init; }
    }

    public class Validator : AbstractValidator<Command> { }
}