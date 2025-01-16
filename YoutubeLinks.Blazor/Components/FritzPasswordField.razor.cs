using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzPasswordField : ComponentBase
{
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private InputType _passwordInputType = InputType.Password;
    private bool _passwordShown;

    [Parameter] public string Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>> For { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public bool OnlyValidateIfDirty { get; set; } = true;
    [Parameter] public Dictionary<string, object> UserAttributes { get; set; }

    private async Task OnValueChanged(string newValue)
    {
        Value = newValue;
        await ValueChanged.InvokeAsync(newValue);
    }

    private void ShowPassword()
    {
        if (_passwordShown)
        {
            _passwordShown = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInputType = InputType.Password;
        }
        else
        {
            _passwordShown = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInputType = InputType.Text;
        }
    }
}