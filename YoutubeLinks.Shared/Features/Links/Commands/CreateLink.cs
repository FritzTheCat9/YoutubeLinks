using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Links.Commands;

public static class CreateLink
{
    public class Command : IRequest<int>
    {
        public string Url { get; set; }
        public int PlaylistId { get; set; }
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