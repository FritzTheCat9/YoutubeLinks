using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzProcessingButton(
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    private bool _processing;

    [Parameter] public string ProcessingButtonText { get; set; }
    [Parameter] public string ButtonText { get; set; }
    [Parameter] public ButtonType ButtonType { get; set; }
    [Parameter] public Color Color { get; set; }
    [Parameter] public Dictionary<string, object> UserAttributes { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(ProcessingButtonText))
        {
            ProcessingButtonText = localizer[nameof(AppStrings.Processing)];
        }
    }

    public void SetProcessing(bool processing)
    {
        _processing = processing;
        StateHasChanged();
    }

    public bool IsProcessing()
    {
        return _processing;
    }
}