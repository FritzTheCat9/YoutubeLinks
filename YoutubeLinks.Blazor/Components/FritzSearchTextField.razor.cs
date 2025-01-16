using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzSearchTextField(
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    [Parameter] public string Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>> For { get; set; }
    [Parameter] public EventCallback<string> OnSearch { get; set; }
    [Parameter] public Dictionary<string, object> UserAttributes { get; set; }

    private async Task OnValueChanged(string newValue)
    {
        Value = newValue;
        await ValueChanged.InvokeAsync(newValue);
        await OnSearch.InvokeAsync(newValue);
    }
}