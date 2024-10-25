using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Links.Commands
{
    public class DownloadSingleLinkTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void DownloadSingleLinkValidator_Url_ShouldNotBeEmpty(string url)
        {
            const string message = "Youtube video url should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.UrlNotEmpty), message);

            var validator = new DownloadSingleLink.Validator(localizer);

            var command = new DownloadSingleLink.Command
            {
                Url = url, 
                YoutubeFileType = YoutubeFileType.Mp3,
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
        public void DownloadSingleLinkValidator_Url_ShouldMatchYoutubeVideoRegex(string url)
        {
            const string message = "This is not a valid link to the YouTube video.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.VideoUrlMatchesRegex), message);

            var validator = new DownloadSingleLink.Validator(localizer);

            var command = new DownloadSingleLink.Command
            {
                Url = url,
                YoutubeFileType = YoutubeFileType.Mp3,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData((YoutubeFileType)2)]
        [InlineData((YoutubeFileType)3)]
        [InlineData((YoutubeFileType)4)]
        [InlineData((YoutubeFileType)5)]
        public void DownloadSingleLinkValidator_YoutubeFileType_ShouldBeInEnum(YoutubeFileType youtubeFileType)
        {
            var message = $"YoutubeFileType has a range of values which does not include: {youtubeFileType}.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.YoutubeFileTypeIsInEnum), message);

            var validator = new DownloadSingleLink.Validator(localizer);

            var command = new DownloadSingleLink.Command
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                YoutubeFileType = youtubeFileType,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.YoutubeFileType)
                  .WithErrorMessage(message);
        }
    }
}
