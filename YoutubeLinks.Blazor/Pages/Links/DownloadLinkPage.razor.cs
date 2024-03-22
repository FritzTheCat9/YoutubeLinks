using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Blazor.Pages.Links
{
    public partial class DownloadLinkPage : ComponentBase
    {
        private CustomValidator _customValidator;
        private List<BreadcrumbItem> _items;
        private FritzProcessingButton _processingButton;

        [Parameter] public DownloadSingleLink.Command Command { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public ILinkApiClient LinkApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override void OnParametersSet()
        {
            _items =
            [
                new(Localizer[nameof(AppStrings.DownloadLink)], href: null, disabled: true),
            ];
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                _processingButton.SetProcessing(true);

                var response = await LinkApiClient.DownloadSingleLink(Command);
                var content = await response.Content.ReadAsByteArrayAsync();
                var filename = response.Content.Headers.ContentDisposition.FileNameStar ?? $"default_name.{YoutubeHelpers.YoutubeFileTypeToString(Command.YoutubeFileType)}";

                await JSRuntime.InvokeVoidAsync("downloadFile", filename, content);

                Command.Url = "";
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