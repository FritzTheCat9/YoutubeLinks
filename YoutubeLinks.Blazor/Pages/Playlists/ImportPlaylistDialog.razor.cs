using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Newtonsoft.Json;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.Blazor.Pages.Playlists
{
    public partial class ImportPlaylistDialog : ComponentBase
    {
        private EditForm _form;
        private CustomValidator _customValidator;

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        [Parameter] public ImportPlaylistFromJson.FormModel FormModel { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IStringLocalizer<ValidationMessage> SharedLocalizer { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
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
        }

        private async Task UploadFile(InputFileChangeEventArgs e)
        {
            var fileValidator = new ImportPlaylistFromJson.FileValidator(SharedLocalizer);
            var validationResult = await fileValidator.ValidateAsync(FormModel.File);
            if (!validationResult.IsValid)
                return;

            var file = e.File;
            if (file is null)
                return;

            var stream = file.OpenReadStream(5242880);
            var fileContent = await new StreamReader(stream).ReadToEndAsync();
            var exportedLinks = JsonConvert.DeserializeObject<ExportPlaylist.PlaylistModel>(fileContent);

            FormModel.Name = file.Name.Split('.')[0];
            FormModel.ExportedLinks = exportedLinks.LinkModels.ToList();

            _form.EditContext.Validate();
        }
    }
}