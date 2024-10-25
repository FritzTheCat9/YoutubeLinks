using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Users.Commands
{
    public class UpdateUserThemeTests
    {
        [Theory]
        [InlineData((ThemeColor)3)]
        [InlineData((ThemeColor)4)]
        [InlineData((ThemeColor)5)]
        public void UpdateUserThemeValidator_YoutubeFileType_ShouldBeInEnum(ThemeColor themeColor)
        {
            var message = $"ThemeColor has a range of values which does not include: {themeColor}.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.ThemeColorIsInEnum), message);

            var validator = new UpdateUserTheme.Validator(localizer);

            var command = new UpdateUserTheme.Command
            {
                Id = 1,
                ThemeColor = themeColor,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ThemeColor)
                  .WithErrorMessage(message);
        }
    }
}
