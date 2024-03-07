using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Links.Commands
{
    public class UpdateLink
    {
        public class Command : IRequest<Unit>
        {
            public int Id { get; set; }
            public string Url { get; set; }
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
            }
        }
    }
}
