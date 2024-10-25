using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Shared;

public partial class SuccessDialog : ComponentBase
{
    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

    [Parameter] public string ContentText { get; set; }
    [Parameter] public string ButtonText { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Success;

    [Inject] public IStringLocalizer<App> Localizer { get; set; }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(ContentText))
            ContentText = Localizer[nameof(AppStrings.Success)];

        if (string.IsNullOrWhiteSpace(ButtonText))
            ButtonText = Localizer[nameof(AppStrings.Ok)];
    }

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }
}