using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Users.Commands
{
    public static class UpdateUserTheme
    {
        public class Command : IRequest<Unit>
        {
            public int Id { get; set; }
            public ThemeColor ThemeColor { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.ThemeColor)
                    .IsInEnum()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.ThemeColorIsInEnum)]);
            }
        }
    }
}
