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

        private List<DownloadLinkResult> _downloadLinkResults = [];

        private class DownloadLinkResult
        {
            public string Url { get; set; }
            public bool Success { get; set; }
        }

        public class DownloadLinkPageConst
        {
            public const string UrlInput = "download-link-page-url-input";
            public const string YoutubeFileTypeSelect = "download-link-page-youtube-file-type-select";
            public const string DownloadButton = "download-link-page-download-button";
        }

        [Parameter]
        public DownloadSingleLink.Command Command { get; set; } = new()
        {
            Url = "",
            YoutubeFileType = YoutubeFileType.MP3
        };

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
                var content = await response.Content.ReadAsStreamAsync();
                var streamRef = new DotNetStreamReference(content);
                var filename = response.Content.Headers.ContentDisposition.FileNameStar ?? $"default_name.{YoutubeHelpers.YoutubeFileTypeToString(Command.YoutubeFileType)}";

                await JSRuntime.InvokeVoidAsync("downloadFile", filename, streamRef);

                _downloadLinkResults.Add(new()
                {
                    Url = Command.Url,
                    Success = true,
                });
            }
            catch (MyValidationException validationException)
            {
                _customValidator.DisplayErrors(validationException.Errors);
            }
            catch (Exception ex)
            {
                _downloadLinkResults.Add(new()
                {
                    Url = Command.Url,
                    Success = false,
                });
            }
            finally
            {
                _processingButton.SetProcessing(false);
                Command.Url = "";
            }
        }
    }
}