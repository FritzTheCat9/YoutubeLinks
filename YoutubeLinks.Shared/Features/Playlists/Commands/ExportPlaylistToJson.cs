using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Playlists.Commands
{
    public class ExportPlaylistToJson
    {
        public class Command : IRequest<PlaylistJsonFile>
        {
            public int Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {

        }

        public class PlaylistJsonFile
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
        }

        public class ExportedLinkModel
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string VideoId { get; set; }
        }
    }
}
