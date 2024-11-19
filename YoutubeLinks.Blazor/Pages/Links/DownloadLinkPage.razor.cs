using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Shared.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Blazor.Pages.Links;

public partial class DownloadLinkPage(
    ILinkApiClient linkApiClient,
    IStringLocalizer<App> localizer,
    IJSRuntime jsRuntime)
    : ComponentBase
{
    private readonly List<DownloadLinkResult> _downloadLinkResults = [];
    private CustomValidator _customValidator;
    private List<BreadcrumbItem> _items;
    private FritzProcessingButton _processingButton;

    [Parameter]
    public DownloadSingleLink.Command Command { get; set; } = new()
    {
        Url = "",
        YoutubeFileType = YoutubeFileType.Mp3
    };

    protected override void OnParametersSet()
    {
        _items =
        [
            new BreadcrumbItem(localizer[nameof(AppStrings.DownloadLink)], null, true)
        ];
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            _processingButton.SetProcessing(true);

            var response = await linkApiClient.DownloadSingleLink(Command);

            await using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var streamRef = new DotNetStreamReference(stream);
                var filename = response.Content.Headers.ContentDisposition?.FileNameStar ??
                               $"default_name.{YoutubeHelpers.YoutubeFileTypeToString(Command.YoutubeFileType)}";

                await jsRuntime.InvokeVoidAsync("downloadFile", filename, streamRef);
            }

            _downloadLinkResults.Add(new DownloadLinkResult
            {
                Url = Command.Url,
                Success = true
            });
        }
        catch (MyValidationException validationException)
        {
            _customValidator.DisplayErrors(validationException.Errors);
        }
        catch (Exception)
        {
            _downloadLinkResults.Add(new DownloadLinkResult
            {
                Url = Command.Url,
                Success = false
            });
        }
        finally
        {
            _processingButton.SetProcessing(false);
            Command.Url = "";
        }
    }

    private class DownloadLinkResult
    {
        public string Url { get; init; }
        public bool Success { get; init; }
    }

    public abstract class DownloadLinkPageConst
    {
        public const string UrlInput = "download-link-page-url-input";
        public const string YoutubeFileTypeSelect = "download-link-page-youtube-file-type-select";
        public const string DownloadButton = "download-link-page-download-button";
    }
}