using Blazored.LocalStorage;
using YoutubeLinks.Shared.Localization.Resources;

namespace YoutubeLinks.Blazor.Localization
{
    public interface ILocalizationProvider
    {
        Task<string> GetCulture();
        Task SetCulture(string culture);
    }

    public class LocalizationProvider : ILocalizationProvider
    {
        private readonly ILocalStorageService _localStorage;

        public LocalizationProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<string> GetCulture()
            => await _localStorage.GetItemAsync<string>(LocalizationConsts.CultureKey);

        public async Task SetCulture(string culture)
            => await _localStorage.SetItemAsync(LocalizationConsts.CultureKey, culture);
    }
}
