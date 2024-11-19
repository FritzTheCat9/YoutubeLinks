using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Newtonsoft.Json;
using YoutubeLinks.Shared.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Blazor.Pages.Playlists;

public partial class ImportPlaylistDialog(
    IExceptionHandler exceptionHandler,
    IPlaylistApiClient playlistApiClient,
    IStringLocalizer<App> localizer,
    IStringLocalizer<ValidationMessage> sharedLocalizer)
    : ComponentBase
{
    private CustomValidator _customValidator;
    private EditForm _form;
    private FritzProcessingButton _processingButton;

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
    [Parameter] public ImportPlaylist.FormModel FormModel { get; set; } = new();

    private async Task HandleValidSubmit()
    {
        try
        {
            _processingButton.SetProcessing(true);

            await playlistApiClient.ImportPlaylistFromJson(FormModel);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (MyValidationException validationException)
        {
            _customValidator.DisplayErrors(validationException.Errors);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
        finally
        {
            _processingButton.SetProcessing(false);
        }
    }

    private async Task UploadFile(InputFileChangeEventArgs e)
    {
        var fileValidator = new ImportPlaylist.FileValidator(sharedLocalizer);
        var validationResult = await fileValidator.ValidateAsync(FormModel.File);
        if (!validationResult.IsValid)
        {
            return;
        }

        var file = e.File;

        switch (file.ContentType)
        {
            case "text/plain":
                await SetTxtFileVariables(file);
                break;
            case "application/json":
                await SetJsonFileVariables(file);
                break;
            default:
                return;
        }

        _form.EditContext?.Validate();
    }

    private async Task SetJsonFileVariables(IBrowserFile file)
    {
        var stream = file.OpenReadStream(5242880);
        var fileContent = await new StreamReader(stream).ReadToEndAsync();
        var exportedLinks = JsonConvert.DeserializeObject<PlaylistJsonModel>(fileContent);

        FormModel.Name = file.Name.Split('.')[0];
        FormModel.ExportedLinks = exportedLinks.LinkModels.ToList();
        FormModel.ExportedLinkUrls = [];
        FormModel.PlaylistFileType = PlaylistFileType.Json;
    }

    private async Task SetTxtFileVariables(IBrowserFile file)
    {
        var stream = file.OpenReadStream(5242880);
        var fileContent = await new StreamReader(stream).ReadToEndAsync();
        var exportedLinkUrls = ReadUrls(fileContent);

        FormModel.Name = file.Name.Split('.')[0];
        FormModel.ExportedLinks = [];
        FormModel.ExportedLinkUrls = exportedLinkUrls;
        FormModel.PlaylistFileType = PlaylistFileType.Txt;
    }

    private static List<string> ReadUrls(string fileContent)
    {
        var lines = fileContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var urls = lines.Select(line => line.Trim()).ToList();

        return urls;
    }

    public abstract class ImportPlaylistDialogConst
    {
        public const string NameInput = "import-playlist-dialog-name-input";
        public const string PublicCheckbox = "import-playlist-dialog-public-checkbox";
        public const string ImportPlaylistButton = "import-playlist-dialog-import-playlist-button";
    }
}