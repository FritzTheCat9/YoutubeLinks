using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzCopyToClipboardButton : ComponentBase
    {
        [Parameter] public string TooltipText { get; set; }
        [Parameter] public string CopiedText { get; set; }

        [Parameter] public Color Color { get; set; }
        [Parameter] public Size Size { get; set; }
        [Parameter] public Dictionary<string, object?> UserAttributes { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override void OnParametersSet()
        {
            if (string.IsNullOrWhiteSpace(TooltipText))
                TooltipText = Localizer[nameof(AppStrings.CopyToClipboard)];
        }

        private async Task CopyTextToClipboard() 
            => await JSRuntime.InvokeVoidAsync("copyToClipboard", CopiedText);
    }
}