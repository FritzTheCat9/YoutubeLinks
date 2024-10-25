using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class DownloadLinkTests
{
    [Theory]
    [InlineData((YoutubeFileType)2)]
    [InlineData((YoutubeFileType)3)]
    [InlineData((YoutubeFileType)4)]
    [InlineData((YoutubeFileType)5)]
    public void DownloadLinkValidator_YoutubeFileType_ShouldBeInEnum(YoutubeFileType youtubeFileType)
    {
        var message = $"YoutubeFileType has a range of values which does not include: {youtubeFileType}.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.YoutubeFileTypeIsInEnum), message);

        var validator = new DownloadLink.Validator(localizer);

        var command = new DownloadLink.Command
        {
            Id = 1,
            YoutubeFileType = youtubeFileType
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.YoutubeFileType)
            .WithErrorMessage(message);
    }
}