using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Users.Commands
{
    public class LoginTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void LoginCommandValidator_Email_ShouldNotBeEmpty(string email)
        {
            var message = "Email should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailNotEmpty), message);

            var validator = new Login.Validator(localizer);

            var command = new Login.Command
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
        public void LoginCommandValidator_Email_ShouldBeShorterThanMaximumStringLength(string email)
        {
            var message = string.Format("The length of email must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, email.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailMaximumLength), message);

            var validator = new Login.Validator(localizer);

            var command = new Login.Command
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
        public void LoginCommandValidator_Email_ShouldBeEmailAddress(string email)
        {
            var message = "Email address is not valid.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailIsEmailAddress), message);

            var validator = new Login.Validator(localizer);

            var command = new Login.Command
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
        public void LoginCommandValidator_Password_ShouldNotBeEmpty(string password)
        {
            var message = "Password should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordNotEmpty), message);

            var validator = new Login.Validator(localizer);

            var command = new Login.Command
            {
                Password = password,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("123456")]
        public void LoginCommandValidator_Password_ShouldBeGreaterThanMinimumStringLength(string password)
        {
            var message = string.Format("The length of password must be at least {0} characters. You entered {1} characters.",
                ValidationConsts.MinimumStringLength, password.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordMinimumLength), message);

            var validator = new Login.Validator(localizer);

            var command = new Login.Command
            {
                Password = password,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("012345678901234567890123456789012345678901234567890")]
        [InlineData("0123456789012345678901234567890123456789012345678901")]
        public void LoginCommandValidator_Password_ShouldBeShorterThanMaximumStringLength(string password)
        {
            var message = string.Format("The length of password must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, password.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordMaximumLength), message);

            var validator = new Login.Validator(localizer);

            var command = new Login.Command
            {
                Password = password,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(message);
        }
    }
}
