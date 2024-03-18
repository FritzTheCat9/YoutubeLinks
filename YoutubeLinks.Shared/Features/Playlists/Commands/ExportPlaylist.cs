using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Playlists.Commands
{
    public class ExportPlaylist
    {
        public class Command : IRequest<PlaylistFile>
        {
            public int Id { get; set; }
            public PlaylistFileType PlaylistFileType { get; set; }
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

        public class PlaylistFile
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public PlaylistFileType PlaylistFileType { get; set; }
            public string ContentType { get; set; }
        }

        public class PlaylistModel
        {
            public int LinksCount { get; set; }
            public IEnumerable<LinkModel> LinkModels { get; set; }
        }

        public class LinkModel
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string VideoId { get; set; }
        }

        public enum PlaylistFileType
        {
            JSON,
            TXT,
        }
    }
}
