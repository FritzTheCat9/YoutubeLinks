using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Shared;

public partial class SuccessDialog(
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }
    [Parameter] public string ContentText { get; set; }
    [Parameter] public string ButtonText { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Success;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(ContentText))
        {
            ContentText = localizer[nameof(AppStrings.Success)];
        }

        if (string.IsNullOrWhiteSpace(ButtonText))
        {
            ButtonText = localizer[nameof(AppStrings.Ok)];
        }
    }

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }
}