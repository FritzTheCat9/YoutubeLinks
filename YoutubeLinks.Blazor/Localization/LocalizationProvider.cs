using Blazored.LocalStorage;
using YoutubeLinks.Shared.Localization.Resources;

namespace YoutubeLinks.Blazor.Localization;

public interface ILocalizationProvider
{
    Task<string> GetCulture();
    Task SetCulture(string culture);
}

public class LocalizationProvider(ILocalStorageService localStorage) : ILocalizationProvider
{
    public async Task<string> GetCulture()
    {
        return await localStorage.GetItemAsync<string>(LocalizationConsts.CultureKey);
    }

    public async Task SetCulture(string culture)
    {
        await localStorage.SetItemAsync(LocalizationConsts.CultureKey, culture);
    }
}