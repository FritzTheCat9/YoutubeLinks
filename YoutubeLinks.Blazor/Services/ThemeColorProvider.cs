using Blazored.LocalStorage;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Blazor.Services
{
    public interface IThemeColorProvider
    {
        Task<ThemeColor> GetThemeColor();
        Task SetThemeColor(ThemeColor themeColor);
    }

    public class ThemeColorProvider : IThemeColorProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly string _themeColor = "ThemeColor";

        public ThemeColorProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<ThemeColor> GetThemeColor()
            => await _localStorageService.GetItemAsync<ThemeColor?>(_themeColor) ?? ThemeColor.System;

        public async Task SetThemeColor(ThemeColor value)
            => await _localStorageService.SetItemAsync(_themeColor, value);
    }
}
