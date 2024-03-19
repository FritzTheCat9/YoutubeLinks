using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzCopyToClipboardButton : MudButton
    {
        [Parameter] public string TooltipText { get; set; }
        [Parameter] public string CopiedText { get; set; }

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