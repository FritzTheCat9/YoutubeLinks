using Blazored.LocalStorage;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Blazor.Services;

public interface IThemeColorProvider
{
    Task<ThemeColor> GetThemeColor();
    Task SetThemeColor(ThemeColor themeColor);
}

public class ThemeColorProvider(ILocalStorageService localStorageService) : IThemeColorProvider
{
    private const string _themeColor = "ThemeColor";

    public async Task<ThemeColor> GetThemeColor()
    {
        return await localStorageService.GetItemAsync<ThemeColor?>(_themeColor) ??
               YoutubeLinks.Shared.Features.Users.Helpers.ThemeColor.System;
    }

    public async Task SetThemeColor(ThemeColor value)
    {
        await localStorageService.SetItemAsync(_themeColor, value);
    }
}