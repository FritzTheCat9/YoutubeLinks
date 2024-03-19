using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzSearchTextField : MudTextField<string>
    {
        [Parameter] public EventCallback<string> OnSearch { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        private async Task OnValueChanged(string value)
        {
            await SetValueAsync(value);
            await OnSearch.InvokeAsync(value);
        }
    }
}