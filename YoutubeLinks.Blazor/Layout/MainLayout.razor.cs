using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Services;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Blazor.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        private bool _drawerOpen = true;

        private MudThemeProvider _mudThemeProvider;
        private ThemeColor _themeColor = ThemeColor.System;
        private bool _isDarkMode = false;
        private string _icon = Icons.Material.Rounded.LightMode;
        private string _text;

        private readonly MudTheme _customTheme = new()
        {
            Palette = new PaletteLight()
            {
                Error = Colors.Red.Default,
                ErrorContrastText = Colors.Red.Default,
                ErrorDarken = Colors.Red.Default,
                ErrorLighten = Colors.Red.Default,
            },
            PaletteDark = new PaletteDark()
            {
                Error = Colors.Red.Default,
                ErrorContrastText = Colors.Red.Default,
                ErrorDarken = Colors.Red.Default,
                ErrorLighten = Colors.Red.Default,
            },
        };

        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IThemeColorProvider ThemeColorProvider { get; set; }

        protected override void OnParametersSet()
        {
            _text = Localizer[nameof(AppStrings.SwitchToLightTheme)];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _themeColor = await ThemeColorProvider.GetThemeColor();

                switch (_themeColor)
                {
                    case ThemeColor.System:
                        await SetSystemMode();
                        break;
                    case ThemeColor.Light:
                        await SetLightMode();
                        break;
                    case ThemeColor.Dark:
                        await SetDarkMode();
                        break;
                }

                await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
            }
        }

        private Task OnSystemPreferenceChanged(bool newValue)
        {
            if (_themeColor != ThemeColor.System)
                return Task.CompletedTask;

            _isDarkMode = newValue;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task OnThemeChange()
        {
            switch (_themeColor)
            {
                case ThemeColor.System:
                    await SetLightMode();
                    break;
                case ThemeColor.Light:
                    await SetDarkMode();
                    break;
                case ThemeColor.Dark:
                    await SetSystemMode();
                    break;
            }
        }

        private async Task SetLightMode()
        {
            await ThemeColorProvider.SetThemeColor(ThemeColor.Light);

            _themeColor = ThemeColor.Light;
            _isDarkMode = false;
            _icon = Icons.Material.Rounded.DarkMode;
            _text = Localizer[nameof(AppStrings.SwitchToDarkTheme)];

            StateHasChanged();
        }

        private async Task SetDarkMode()
        {
            await ThemeColorProvider.SetThemeColor(ThemeColor.Dark);

            _themeColor = ThemeColor.Dark;
            _isDarkMode = true;
            _icon = Icons.Material.Rounded.SettingsBrightness;
            _text = Localizer[nameof(AppStrings.SwitchToSystemTheme)];

            StateHasChanged();
        }

        private async Task SetSystemMode()
        {
            await ThemeColorProvider.SetThemeColor(ThemeColor.System);

            _themeColor = ThemeColor.System;
            _isDarkMode = await _mudThemeProvider.GetSystemPreference();
            _icon = Icons.Material.Rounded.LightMode;
            _text = Localizer[nameof(AppStrings.SwitchToLightTheme)];

            StateHasChanged();
        }

        private void DrawerToggle()
            => _drawerOpen = !_drawerOpen;
    }
}