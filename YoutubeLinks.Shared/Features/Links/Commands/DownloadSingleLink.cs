using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public class DownloadSingleLink
    {
        public class Command : IRequest<Response>
        {
            public string Url { get; set; }
            public YoutubeFileType YoutubeFileType { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.Url)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.UrlNotEmpty)])
                    .Matches(ValidationConsts.YoutubeVideoUrlRegex)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.VideoUrlMatchesRegex)]);

                RuleFor(x => x.YoutubeFileType)
                    .IsInEnum()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.YoutubeFileTypeIsInEnum)]);
            }
        }

        public class Response
        {
            public byte[] FileBytes { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
        }

        public enum YoutubeFileType
        {
            MP3,
            MP4,
        }
    }
}
