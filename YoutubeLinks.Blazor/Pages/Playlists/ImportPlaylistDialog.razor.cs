using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Newtonsoft.Json;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Blazor.Pages.Playlists
{
    public partial class ImportPlaylistDialog : ComponentBase
    {
        private EditForm _form;
        private CustomValidator _customValidator;
        private FritzProcessingButton _processingButton;


        public class ImportPlaylistDialogConst
        {
            public const string NameInput = "import-playlist-dialog-name-input";
            public const string PublicCheckbox = "import-playlist-dialog-public-checkbox";
            public const string ImportPlaylistButton = "import-playlist-dialog-import-playlist-button";
        }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        [Parameter] public ImportPlaylist.FormModel FormModel { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IStringLocalizer<ValidationMessage> SharedLocalizer { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
                _processingButton.SetProcessing(true);

                await PlaylistApiClient.ImportPlaylistFromJson(FormModel);
                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (MyValidationException validationException)
            {
                _customValidator.DisplayErrors(validationException.Errors);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
            finally
            {
                _processingButton.SetProcessing(false);
            }
        }

        private async Task UploadFile(InputFileChangeEventArgs e)
        {
            var fileValidator = new ImportPlaylist.FileValidator(SharedLocalizer);
            var validationResult = await fileValidator.ValidateAsync(FormModel.File);
            if (!validationResult.IsValid)
                return;

            var file = e.File;
            if (file is null)
                return;

            switch (file.ContentType)
            {
                case "text/plain":
                    await SetTXTFileVariables(file);
                    break;
                case "application/json":
                    await SetJSONFileVariables(file);
                    break;
                default:
                    return;
            }

            _form.EditContext.Validate();
        }

        private async Task SetJSONFileVariables(IBrowserFile file)
        {
            var stream = file.OpenReadStream(5242880);
            var fileContent = await new StreamReader(stream).ReadToEndAsync();
            var exportedLinks = JsonConvert.DeserializeObject<PlaylistJSONModel>(fileContent);

            FormModel.Name = file.Name.Split('.')[0];
            FormModel.ExportedLinks = exportedLinks.LinkModels.ToList();
            FormModel.ExportedLinkUrls = [];
            FormModel.PlaylistFileType = PlaylistFileType.JSON;
        }

        private async Task SetTXTFileVariables(IBrowserFile file)
        {
            var stream = file.OpenReadStream(5242880);
            var fileContent = await new StreamReader(stream).ReadToEndAsync();
            var exportedLinkUrls = ReadUrls(fileContent);

            FormModel.Name = file.Name.Split('.')[0];
            FormModel.ExportedLinks = [];
            FormModel.ExportedLinkUrls = exportedLinkUrls;
            FormModel.PlaylistFileType = PlaylistFileType.TXT;
        }

        private static List<string> ReadUrls(string fileContent)
        {
            var urls = new List<string>();
            var lines = fileContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                urls.Add(line.Trim());
            }

            return urls;
        }
    }
}