using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Shared.Localization;

namespace YoutubeLinks.Shared.Features.Users.Commands
{
    public class ResetPassword
    {
        public class Command : IRequest<bool>
        {
            public string Email { get; set; }
            public string Token { get; set; }
            public string Password { get; set; }
            public string RepeatPassword { get; set; }
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

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.PasswordNotEmpty)])
                    .MinimumLength(ValidationConsts.MinimumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.PasswordMinimumLength)])
                    .MaximumLength(ValidationConsts.MaximumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.PasswordMaximumLength)]);

                RuleFor(x => x.RepeatPassword)
                    .NotEmpty()
                    .WithMessage(x => localizer[nameof(ValidationMessageString.PasswordNotEmpty)])
                    .MinimumLength(ValidationConsts.MinimumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.PasswordMinimumLength)])
                    .MaximumLength(ValidationConsts.MaximumStringLength)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.PasswordMaximumLength)])
                    .Equal(x => x.Password)
                    .WithMessage(x => localizer[nameof(ValidationMessageString.RepeatPasswordEqualPassword)]);
            }
        }
    }
}
