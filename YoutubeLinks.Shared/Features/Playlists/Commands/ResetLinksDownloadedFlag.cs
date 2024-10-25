using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Playlists.Commands;

public static class ResetLinksDownloadedFlag
{
    public class Command : IRequest<Unit>
    {
        public int Id { get; set; }
        public bool IsDownloaded { get; set; }
    }

    public class Validator : AbstractValidator<Command> { }
}