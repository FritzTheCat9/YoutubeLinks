using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Users.Commands
{
    public class ConfirmEmailTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void ConfirmEmailCommandValidator_Email_ShouldNotBeEmpty(string email)
        {
            var message = "Email should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailNotEmpty), message);

            var validator = new ConfirmEmail.Validator(localizer);

            var command = new ConfirmEmail.Command
            {
                Email = email,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("012345678901234567890123456789012345678901234567890")]
        [InlineData("0123456789012345678901234567890123456789012345678901")]
        public void ConfirmEmailCommandValidator_Email_ShouldBeShorterThanMaximumStringLength(string email)
        {
            var message = string.Format("The length of email must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, email.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailMaximumLength), message);

            var validator = new ConfirmEmail.Validator(localizer);

            var command = new ConfirmEmail.Command
            {
                Email = email,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("@a")]
        [InlineData("a@")]
        [InlineData("a.com")]
        [InlineData("https://google.com")]
        public void ConfirmEmailCommandValidator_Email_ShouldBeEmailAddress(string email)
        {
            var message = "Email address is not valid.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailIsEmailAddress), message);

            var validator = new ConfirmEmail.Validator(localizer);

            var command = new ConfirmEmail.Command
            {
                Email = email,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void ConfirmEmailCommandValidator_Token_ShouldNotBeEmpty(string token)
        {
            var message = "Token should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.TokenNotEmpty), message);

            var validator = new ConfirmEmail.Validator(localizer);

            var command = new ConfirmEmail.Command
            {
                Token = token,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Token)
                  .WithErrorMessage(message);
        }
    }
}
