using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.Blazor.Pages.Links
{
    public partial class UpdateLinkDialog : ComponentBase
    {
        private CustomValidator _customValidator;
        private FritzProcessingButton _processingButton;

        public class UpdateLinkDialogConst
        {
            public const string UrlInput = "update-link-dialog-url-input";
            public const string TitleInput = "update-link-dialog-title-input";
            public const string DownloadedCheckbox = "update-link-dialog-downloaded-checkbox";
            public const string UpdateButton = "update-link-dialog-update-button";
        }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        [Parameter] public UpdateLink.Command Command { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public ILinkApiClient LinkApiClient { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
                _processingButton.SetProcessing(true);

                await LinkApiClient.UpdateLink(Command);

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
    }
}