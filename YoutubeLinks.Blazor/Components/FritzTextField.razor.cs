using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzTextField : ComponentBase
{
    [Parameter] public string Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>> For { get; set; }

    [Parameter] public string Label { get; set; }
    [Parameter] public bool HideField { get; set; }
    [Parameter] public Variant Variant { get; set; }
    [Parameter] public bool ShrinkLabel { get; set; }
    [Parameter] public Dictionary<string, object?> UserAttributes { get; set; }

    private async Task OnValueChanged(string newValue)
    {
        Value = newValue;
        await ValueChanged.InvokeAsync(newValue);
    }
}