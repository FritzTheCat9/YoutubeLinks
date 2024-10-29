using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Playlists.Commands;

public static class ExportPlaylist
{
    public class Command : IRequest<PlaylistFile>
    {
        public int Id { get; init; }
        public PlaylistFileType PlaylistFileType { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IStringLocalizer<ValidationMessage> localizer)
        {
            RuleFor(x => x.PlaylistFileType)
                .IsInEnum()
                .WithMessage(x => localizer[nameof(ValidationMessageString.PlaylistFileTypeIsInEnum)]);
        }
    }
}