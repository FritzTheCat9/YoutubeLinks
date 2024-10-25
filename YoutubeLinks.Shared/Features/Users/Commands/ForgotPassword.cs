using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Users.Commands
{
    public static class ForgotPassword
    {
        public class Command : IRequest<Unit>
        {
            public string Email { get; set; }
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
            }
        }
    }
}
