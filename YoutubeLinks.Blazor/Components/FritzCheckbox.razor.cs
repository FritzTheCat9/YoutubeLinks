using MudBlazor;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzCheckbox : MudCheckBox<bool>
    {
        private void OnValueChanged(bool value) 
            => SetBoolValueAsync(value);
    }
}