using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Shared;

public partial class InformationDialog(
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }
    [Parameter] public string ContentText { get; set; }
    [Parameter] public string ButtonText { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Primary;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(ContentText))
        {
            ContentText = localizer[nameof(AppStrings.InformationConfirmInfo)];
        }

        if (string.IsNullOrWhiteSpace(ButtonText))
        {
            ButtonText = localizer[nameof(AppStrings.Yes)];
        }
    }

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    public abstract class InformationDialogConst
    {
        public const string ConfirmButton = "information-dialog-confirm-button";
    }
}