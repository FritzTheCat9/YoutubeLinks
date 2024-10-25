using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public static class DownloadSingleLink
    {
        public class Command : IRequest<YoutubeFile>
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
    }
}
