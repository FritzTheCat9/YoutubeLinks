using FluentValidation.TestHelper;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands
{
    public class CreatePlaylistTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("   ")]
        public void CreatePlaylistCommandValidator_Name_ShouldNotBeEmpty(string name)
        {
            const string message = "Name should not be empty.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.NameNotEmpty), message);

            var validator = new CreatePlaylist.Validator(localizer);

            var command = new CreatePlaylist.Command
            {
                Name = name,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(message);
        }

        [Theory]
        [InlineData("012345678901234567890123456789012345678901234567890")]
        [InlineData("0123456789012345678901234567890123456789012345678901")]
        public void CreatePlaylistCommandValidator_Name_ShouldBeShorterThanMaximumStringLength(string name)
        {
            var message =
                $"The length of name must be {ValidationConsts.MaximumStringLength} characters or fewer. You entered {name.Length} characters.";

            var localizer = new TestStringLocalizer<ValidationMessage>();
            localizer.AddTranslation(nameof(ValidationMessageString.NameMaximumLength), message);

            var validator = new CreatePlaylist.Validator(localizer);

            var command = new CreatePlaylist.Command
            {
                Name = name,
            };

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(message);
        }
    }
}
