using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YoutubeLinks.Shared.Localization.Resources;

namespace YoutubeLinks.Blazor.Localization;

public static class LocalizationExtensions
{
    public static IServiceCollection AddMyLocalization(this IServiceCollection services)
    {
        services.AddLocalization(x => x.ResourcesPath = LocalizationConsts.ResourcesFolder);
        services.AddScoped<ILocalizationProvider, LocalizationProvider>();

        return services;
    }

    public static async Task UseMyLocalization(this WebAssemblyHost host)
    {
        var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
        var cultureString = await localStorage.GetItemAsync<string>(LocalizationConsts.CultureKey);

        CultureInfo cultureInfo;

        if (!string.IsNullOrWhiteSpace(cultureString)
            && LocalizationConsts.SupportedCultures.Any(x => x.CultureInfo.Name == cultureString))
        {
            cultureInfo = new CultureInfo(cultureString);
        }
        else
        {
            cultureInfo = new CultureInfo(LocalizationConsts.DefaultCulture);
            await localStorage.SetItemAsync(LocalizationConsts.CultureKey, LocalizationConsts.DefaultCulture);
        }

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}