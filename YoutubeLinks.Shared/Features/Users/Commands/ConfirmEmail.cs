using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Users.Commands
{
    public static class ConfirmEmail
    {
        public class Command : IRequest<bool>
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(IStringLocalizer<ValidationMessage> localizer)
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.EmailNotEmpty)])
                    .MaximumLength(ValidationConsts.MaximumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.EmailMaximumLength)])
                    .EmailAddress()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.EmailIsEmailAddress)]);

                RuleFor(x => x.Token)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.TokenNotEmpty)]);
            }
        }
    }
}
