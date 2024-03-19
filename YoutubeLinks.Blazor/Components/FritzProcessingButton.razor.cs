using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzProcessingButton : MudButton
    {
        private bool _processing;

        [Parameter] public string ProcessingButtonText { get; set; }
        [Parameter] public string ButtonText { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        protected override void OnParametersSet()
        {
            if (string.IsNullOrWhiteSpace(ProcessingButtonText))
                ProcessingButtonText = Localizer[nameof(AppStrings.Processing)];
        }

        public void SetProcessing(bool processing)
        {
            _processing = processing;
            StateHasChanged();
        }

        public bool IsProcessing() 
            => _processing;
    }
}