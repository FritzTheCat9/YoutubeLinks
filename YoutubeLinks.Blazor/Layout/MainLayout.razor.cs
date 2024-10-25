using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Services;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Blazor.Layout;

public partial class MainLayout : LayoutComponentBase
{
    private readonly MudTheme _customTheme = new()
    {
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        },
        PaletteLight = new PaletteLight
        {
            Error = Colors.Red.Default,
            ErrorContrastText = Colors.Red.Default,
            ErrorDarken = Colors.Red.Default,
            ErrorLighten = Colors.Red.Default,

            AppbarBackground = "80BCBD",
            DrawerBackground = "AAD9BB",
            Background = "D5F0C1",
            TableLines = "D5F0C1",
            Surface = "F9F7C9",
            Primary = "9A3E88",
            Warning = Colors.Yellow.Darken4,

            AppbarText = Colors.Gray.Darken4,
            DrawerIcon = Colors.Gray.Darken3,

            TextPrimary = Colors.Gray.Darken3,
            TextSecondary = Colors.Gray.Darken2
        },
        PaletteDark = new PaletteDark
        {
            Error = Colors.Red.Default,
            ErrorContrastText = Colors.Red.Default,
            ErrorDarken = Colors.Red.Default,
            ErrorLighten = Colors.Red.Default
        }
    };

    private bool _drawerOpen = true;
    private string _icon = Icons.Material.Rounded.LightMode;
    private bool _isDarkMode;
    private MudThemeProvider _mudThemeProvider;
    private string _text;
    private ThemeColor _themeColor = ThemeColor.System;

    public int? UserId { get; set; }

    [Inject] public IExceptionHandler ExceptionHandler { get; set; }
    [Inject] public IUserApiClient UserApiClient { get; set; }

    [Inject] public IAuthService AuthService { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }
    [Inject] public IThemeColorProvider ThemeColorProvider { get; set; }
    [Inject] public TokenRefreshService TokenRefreshService { get; set; }

    public async Task UserChanged()
    {
        UserId = await AuthService.GetCurrentUserId();
    }

    protected override void OnParametersSet()
    {
        _text = Localizer[nameof(AppStrings.SwitchToLightTheme)];
    }

    protected override async Task OnInitializedAsync()
    {
        await TokenRefreshService.RefreshToken();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _themeColor = await ThemeColorProvider.GetThemeColor();

            await RefreshThemeColor();

            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
        }
    }

    public async Task ChangeThemeColor(ThemeColor themeColor)
    {
        _themeColor = themeColor;
        await RefreshThemeColor();
    }

    private async Task RefreshThemeColor()
    {
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
    }

    private Task OnSystemPreferenceChanged(bool newValue)
    {
        if (_themeColor != ThemeColor.System)
        {
            return Task.CompletedTask;
        }

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

        await UpdateLoggedUserTheme();
    }

    private async Task UpdateLoggedUserTheme()
    {
        var userId = await AuthService.GetCurrentUserId();
        if (userId is null)
        {
            return;
        }

        try
        {
            var command = new UpdateUserTheme.Command
            {
                Id = userId.Value,
                ThemeColor = _themeColor
            };

            await UserApiClient.UpdateUserTheme(command);
        }
        catch (Exception ex)
        {
            ExceptionHandler.HandleExceptions(ex);
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
    {
        _drawerOpen = !_drawerOpen;
    }

    public class MainLayoutConst
    {
        public const string ToggleNavMenuButton = "main-layout-toogle-nav-menu-button";
        public const string ChangeThemeButton = "main-layout-change-theme-button";
        public const string RedirectToProjectGithubPageButton = "main-layout-redirect-to-project-github-page-button";
    }
}