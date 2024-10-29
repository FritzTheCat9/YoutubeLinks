using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzCopyToClipboardButton(
    IStringLocalizer<App> localizer,
    IJSRuntime jsRuntime)
    : ComponentBase
{
    [Parameter] public string TooltipText { get; set; }
    [Parameter] public string CopiedText { get; set; }
    [Parameter] public Color Color { get; set; }
    [Parameter] public Size Size { get; set; }
    [Parameter] public Dictionary<string, object?> UserAttributes { get; set; }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(TooltipText))
        {
            TooltipText = localizer[nameof(AppStrings.CopyToClipboard)];
        }
    }

    private async Task CopyTextToClipboard()
    {
        await jsRuntime.InvokeVoidAsync("copyToClipboard", CopiedText);
    }
}