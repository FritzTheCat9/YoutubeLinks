using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Links.Commands
{
    public class UpdateLinkTestsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void UpdateLinkValidator_Url_ShouldNotBeEmpty(string url)
        {
            var message = "Youtube video url should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.UrlNotEmpty), message);

            var validator = new UpdateLink.Validator(localizer);

            var command = new UpdateLink.Command
            {
                Id = 1,
                Url = url,
                Downloaded = true,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("asd")]
        [InlineData("test.test.com")]
        [InlineData("https://www.google.com")]
        [InlineData("https://www.youtube.com/watch?v=")]
        [InlineData("https://www.youtube.com/watch?v=asd")]
        [InlineData("https://www.youtube.com/watch")]
        public void UpdateLinkValidator_Url_ShouldMatchYoutubeVideoRegex(string url)
        {
            var message = "This is not a valid link to the YouTube video.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.VideoUrlMatchesRegex), message);

            var validator = new UpdateLink.Validator(localizer);

            var command = new UpdateLink.Command
            {
                Id = 1,
                Url = url,
                Downloaded = true,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("Title \\")]
        [InlineData("Title ////")]
        [InlineData("Title :")]
        [InlineData("Title *")]
        [InlineData("Title ?")]
        [InlineData("Title \"\"")]
        [InlineData("Title <")]
        [InlineData("Title >")]
        [InlineData("Title |")]
        public void UpdateLinkValidator_Title_ShouldHaveValidCharacters(string title)
        {
            var message = "Title contains invalid characters.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.TitleHaveValidCharacters), message);

            var validator = new UpdateLink.Validator(localizer);

            var command = new UpdateLink.Command
            {
                Id = 1,
                Title = title,
                Downloaded = true,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345")]
        public void UpdateLinkValidator_Title_ShouldHaveLengthShorterThanMaximum(string title)
        {
            var message = string.Format("The length of name must be {0} characters or fewer. You entered {1} characters.",
                YoutubeHelpers.MaximumTitleLength, title.Length);

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.TitleMaximumLength), message);

            var validator = new UpdateLink.Validator(localizer);

            var command = new UpdateLink.Command
            {
                Id = 1,
                Title = title,
                Downloaded = true,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(message);
        }
    }
}
