using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public static class DownloadLink
    {
        public class Command : IRequest<YoutubeFile>
        {
            public int Id { get; set; }
            public YoutubeFileType YoutubeFileType { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.YoutubeFileType)
                    .IsInEnum()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.YoutubeFileTypeIsInEnum)]);
            }
        }
    }
}
