using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands;

public class UpdatePlaylistTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("   ")]
    public void Validator_Should_HaveError_WhenNameIsEmpty(string name)
    {
        const string expectedMessage = "Name should not be empty.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.NameNotEmpty), expectedMessage);

        var validator = new UpdatePlaylist.Validator(localizer);

        var command = new UpdatePlaylist.Command { Name = name };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(expectedMessage);
    }

    [Theory]
    [InlineData("012345678901234567890123456789012345678901234567890")]
    [InlineData("0123456789012345678901234567890123456789012345678901")]
    public void Validator_Should_HaveError_WhenNameExceedsMaxLength(string name)
    {
        var expectedMessage =
            $"The length of name must be {ValidationConsts.MaximumStringLength} characters or fewer. You entered {name.Length} characters.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.NameMaximumLength), expectedMessage);

        var validator = new UpdatePlaylist.Validator(localizer);

        var command = new UpdatePlaylist.Command { Name = name };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage(expectedMessage);
    }
}
