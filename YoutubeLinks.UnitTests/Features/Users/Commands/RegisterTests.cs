﻿using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Users.Commands
{
    public class RegisterTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void RegisterCommandValidator_Email_ShouldNotBeEmpty(string email)
        {
            var message = "Email should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailNotEmpty), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
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
        public void RegisterCommandValidator_Email_ShouldBeShorterThanMaximumStringLength(string email)
        {
            var message = string.Format("The length of email must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, email.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailMaximumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
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
        public void RegisterCommandValidator_Email_ShouldBeEmailAddress(string email)
        {
            var message = "Email address is not valid.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.EmailIsEmailAddress), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
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
        public void RegisterCommandValidator_UserName_ShouldNotBeEmpty(string userName)
        {
            var message = "User name should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.UserNameNotEmpty), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                UserName = userName,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("123456")]
        public void RegisterCommandValidator_UserName_ShouldBeGreaterThanMinimumStringLength(string userName)
        {
            var message = string.Format("The length of password must be at least {0} characters. You entered {1} characters.",
                ValidationConsts.MinimumStringLength, userName.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.UserNameMinimumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                UserName = userName,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("012345678901234567890123456789012345678901234567890")]
        [InlineData("0123456789012345678901234567890123456789012345678901")]
        public void RegisterCommandValidator_UserName_ShouldBeShorterThanMaximumStringLength(string userName)
        {
            var message = string.Format("The length of user name must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, userName.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.UserNameMaximumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                UserName = userName,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("Name!")]
        [InlineData("123asd$")]
        [InlineData("123 asd ASD")]
        [InlineData("123 Test %^&*")]
        public void RegisterCommandValidator_UserName_ShouldMatchUserNameRegex(string userName)
        {
            var message = "UserName can contain only: a-z, A-Z, 0-9 and _ characters.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.UserNameMatchesRegex), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                UserName = userName,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void RegisterCommandValidator_Password_ShouldNotBeEmpty(string password)
        {
            var message = "Password should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordNotEmpty), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
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
        public void RegisterCommandValidator_Password_ShouldBeGreaterThanMinimumStringLength(string password)
        {
            var message = string.Format("The length of password must be at least {0} characters. You entered {1} characters.",
                ValidationConsts.MinimumStringLength, password.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordMinimumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
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
        public void RegisterCommandValidator_Password_ShouldBeShorterThanMaximumStringLength(string password)
        {
            var message = string.Format("The length of password must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, password.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordMaximumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                Password = password,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void RegisterCommandValidator_RepeatPassword_ShouldNotBeEmpty(string repeatPassword)
        {
            var message = "Password should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordNotEmpty), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                RepeatPassword = repeatPassword,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RepeatPassword)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("123456")]
        public void RegisterCommandValidator_RepeatPassword_ShouldBeGreaterThanMinimumStringLength(string repeatPassword)
        {
            var message = string.Format("The length of password must be at least {0} characters. You entered {1} characters.",
                ValidationConsts.MinimumStringLength, repeatPassword.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordMinimumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                RepeatPassword = repeatPassword,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RepeatPassword)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("012345678901234567890123456789012345678901234567890")]
        [InlineData("0123456789012345678901234567890123456789012345678901")]
        public void RegisterCommandValidator_RepeatPassword_ShouldBeShorterThanMaximumStringLength(string repeatPassword)
        {
            var message = string.Format("The length of password must be {0} characters or fewer. You entered {1} characters.",
                ValidationConsts.MaximumStringLength, repeatPassword.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.PasswordMaximumLength), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                RepeatPassword = repeatPassword,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RepeatPassword)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("123", "1234")]
        [InlineData("123", "12")]
        [InlineData("123", "124")]
        [InlineData("password", "not the same password")]
        public void RegisterCommandValidator_RepeatPassword_ShouldBeEqualToPassword(string password, string repeatPassword)
        {
            var message = "The passwords entered must match.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.RepeatPasswordEqualPassword), message);

            var validator = new Register.Validator(localizer);

            var command = new Register.Command
            {
                Password = password,
                RepeatPassword = repeatPassword,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RepeatPassword)
                  .WithErrorMessage(message);
        }
    }
}
