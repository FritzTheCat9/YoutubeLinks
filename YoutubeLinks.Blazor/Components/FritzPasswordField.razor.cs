using MudBlazor;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzPasswordField : MudTextField<string>
    {
        private bool _passwordShown;
        private InputType _passwordInputType = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void OnValueChanged(string value)
            => SetValueAsync(value);

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
}
