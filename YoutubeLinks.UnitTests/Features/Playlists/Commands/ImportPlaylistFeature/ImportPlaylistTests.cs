using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Components.Forms;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Localization;
using YoutubeLinks.UnitTests.Localization;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands.ImportPlaylistFeature;

public class ImportPlaylistTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("   ")]
    public void ImportPlaylistCommandValidator_Name_ShouldNotBeEmpty(string name)
    {
        const string message = "Name should not be empty.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.NameNotEmpty), message);

        var validator = new ImportPlaylist.Validator(localizer);

        var command = new ImportPlaylist.Command
        {
            Name = name
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(message);
    }

    [Theory]
    [InlineData("012345678901234567890123456789012345678901234567890")]
    [InlineData("0123456789012345678901234567890123456789012345678901")]
    public void ImportPlaylistCommandValidator_Name_ShouldBeShorterThanMaximumStringLength(string name)
    {
        var message =
            $"The length of name must be {ValidationConsts.MaximumStringLength} characters or fewer. You entered {name.Length} characters.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.NameMaximumLength), message);

        var validator = new ImportPlaylist.Validator(localizer);

        var command = new ImportPlaylist.Command
        {
            Name = name
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("   ")]
    public void ImportPlaylistFormModelValidator_Name_ShouldNotBeEmpty(string name)
    {
        const string message = "Name should not be empty.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.NameNotEmpty), message);

        var validator = new ImportPlaylist.FormModelValidator(localizer);

        var command = new ImportPlaylist.FormModel
        {
            Name = name
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(message);
    }

    [Theory]
    [InlineData("012345678901234567890123456789012345678901234567890")]
    [InlineData("0123456789012345678901234567890123456789012345678901")]
    public void ImportPlaylistFormModelValidator_Name_ShouldBeShorterThanMaximumStringLength(string name)
    {
        var message =
            $"The length of name must be {ValidationConsts.MaximumStringLength} characters or fewer. You entered {name.Length} characters.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.NameMaximumLength), message);

        var validator = new ImportPlaylist.FormModelValidator(localizer);

        var command = new ImportPlaylist.FormModel
        {
            Name = name
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(message);
    }

    [Fact]
    public void ImportPlaylistFormModelValidator_File_ShouldNotBeEmpty()
    {
        const string message = "Select file.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.FileNotEmpty), message);

        var validator = new ImportPlaylist.FormModelValidator(localizer);

        var command = new ImportPlaylist.FormModel
        {
            Name = "Test Playlist",
            File = null
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage(message);
    }

    [Fact]
    public void ImportPlaylistFormModelValidator_FileSize_ShouldBeLessThanMaxFileSize()
    {
        const int maxFileSize = 5242880;
        var mockedFile = new MockBrowserFile("Test.json", maxFileSize + 1, "application/json");
        var message =
            $"The file size must be less than or equal to {maxFileSize} bytes. The added file have {mockedFile.Size} bytes.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.FileMaxFileSize), message);

        var validator = new ImportPlaylist.FormModelValidator(localizer);

        var command = new ImportPlaylist.FormModel
        {
            Name = "Test Playlist",
            File = mockedFile
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.File.Size)
            .WithErrorMessage(message);
    }

    [Theory]
    [InlineData("png", "image/png")]
    [InlineData("pdf", "application/pdf")]
    [InlineData("wav", "audio/wav")]
    [InlineData("xml", "application/xml")]
    public void ImportPlaylistFormModelValidator_FileContentType_ShouldBeJsonOrTxt(string extension, string type)
    {
        const int maxFileSize = 5242880;
        var mockedFile = new MockBrowserFile($"Test.{extension}", maxFileSize, type);
        const string message = "The file should be in .json or .txt format.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.FileContentTypeShouldBeJsonOrTxt), message);

        var validator = new ImportPlaylist.FormModelValidator(localizer);

        var command = new ImportPlaylist.FormModel
        {
            Name = "Test Playlist",
            File = mockedFile
        };

        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.File.ContentType)
            .WithErrorMessage(message);
    }

    [Fact]
    public void ImportPlaylistFileValidator_Size_ShouldBeLessThanMaxFileSize()
    {
        const int maxFileSize = 5242880;
        var mockedFile = new MockBrowserFile("Test.json", maxFileSize + 1, "application/json");
        var message =
            $"The file size must be less than or equal to {maxFileSize} bytes. The added file have {mockedFile.Size} bytes.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.FileMaxFileSize), message);

        var validator = new ImportPlaylist.FileValidator(localizer);

        var result = validator.TestValidate(mockedFile);

        result.ShouldHaveValidationErrorFor(x => x.Size)
            .WithErrorMessage(message);
    }

    [Theory]
    [InlineData("png", "image/png")]
    [InlineData("pdf", "application/pdf")]
    [InlineData("wav", "audio/wav")]
    [InlineData("xml", "application/xml")]
    public void ImportPlaylistFileValidator_ContentType_ShouldBeJsonOrTxt(string extension, string type)
    {
        const int maxFileSize = 5242880;
        var mockedFile = new MockBrowserFile($"Test.{extension}", maxFileSize, type);
        const string message = "The file should be in .json or .txt format.";

        var localizer = new TestStringLocalizer<ValidationMessage>();
        localizer.AddTranslation(nameof(ValidationMessageString.FileContentTypeShouldBeJsonOrTxt), message);

        var validator = new ImportPlaylist.FileValidator(localizer);

        var result = validator.TestValidate(mockedFile);

        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage(message);
    }

    private class MockBrowserFile(string name, long size, string contentType) : IBrowserFile
    {
        public string Name { get; } = name;
        public long Size { get; } = size;
        public string ContentType { get; } = contentType;
        public DateTimeOffset LastModified { get; }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return new MemoryStream();
        }
    }
}