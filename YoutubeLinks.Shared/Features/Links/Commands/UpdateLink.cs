using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public static class UpdateLink
    {
        public class Command : IRequest<Unit>
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public bool Downloaded { get; set; }
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

                When(x => !string.IsNullOrWhiteSpace(x.Title), () =>
                {
                    RuleFor(x => x.Title)
                        .Must(YoutubeHelpers.HaveValidCharactersInTitle)
                        .WithMessage(x => localizer[nameof(ValidationMessageString.TitleHaveValidCharacters)])
                        .MaximumLength(YoutubeHelpers.MaximumTitleLength)
                        .WithMessage(x => localizer[nameof(ValidationMessageString.TitleMaximumLength)]);
                });
            }
        }
    }
}
