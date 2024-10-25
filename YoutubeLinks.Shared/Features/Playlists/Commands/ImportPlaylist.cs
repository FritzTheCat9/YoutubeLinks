using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Playlists.Commands
{
    public static class ImportPlaylist
    {
        public class Command : IRequest<int>
        {
            public string Name { get; set; }
            public bool Public { get; set; }
            public List<LinkJsonModel> ExportedLinks { get; set; }
            public List<string> ExportedLinkUrls { get; set; }
            public PlaylistFileType PlaylistFileType { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.NameNotEmpty)])
                    .MaximumLength(ValidationConsts.MaximumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.NameMaximumLength)]);
            }
        }

        public class FormModel : Command
        {
            public IBrowserFile File { get; set; }
        }

        public class FormModelValidator : AbstractValidator<FormModel>
        {
            public FormModelValidator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.NameNotEmpty)])
                    .MaximumLength(ValidationConsts.MaximumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.NameMaximumLength)]);

                RuleFor(x => x.File)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.FileNotEmpty)])
                    .SetValidator(new FileValidator(localizer));
            }
        }

        public class FileValidator : AbstractValidator<IBrowserFile>
        {
            private const int MaxFileSize = 5242880;
            private readonly List<string> _allowedFileTypes = ["application/json", "text/plain"];

            public FileValidator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.Size)
                    .LessThanOrEqualTo(MaxFileSize)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.FileMaxFileSize)]);

                RuleFor(x => x.ContentType)
                    .Must(x => _allowedFileTypes.Contains(x))
                    .WithMessage(x => localizer[nameof(ValidationMessageString.FileContentTypeShouldBeJsonOrTxt)]);
            }
        }
    }
}
