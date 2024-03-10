using FluentValidation;
using MediatR;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public class DownloadLink
    {
        public class Command : IRequest<Response>
        {
            public int Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {

        }

        public class Response
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
        }
    }
}
