using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class CreateLinkTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("   ")]
    public void CreateLinkValidator_Url_ShouldNotBeEmpty(string url)
    {
        const string message = "Youtube video url should not be empty.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.UrlNotEmpty), message);

        var validator = new CreateLink.Validator(localizer);

        var command = new CreateLink.Command
        {
            Url = url,
            PlaylistId = 1
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Url)
            .WithErrorMessage(message);
    }

    [Theory]
    [InlineData("asd")]
    [InlineData("www.google.com")]
    [InlineData("test.test.com")]
    [InlineData("https://www.youtube.com/watch?v=")]
    [InlineData("https://www.youtube.com/watch?v=asd")]
    [InlineData("https://www.youtube.com/watch")]
    public void CreateLinkValidator_Url_ShouldMatchYoutubeVideoRegex(string url)
    {
        const string message = "This is not a valid link to the YouTube video.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.VideoUrlMatchesRegex), message);

        var validator = new CreateLink.Validator(localizer);

        var command = new CreateLink.Command
        {
            Url = url,
            PlaylistId = 1
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Url)
            .WithErrorMessage(message);
    }
}