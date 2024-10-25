using YoutubeLinks.Shared.Localization.Resources;

namespace YoutubeLinks.Api.Localization;

public static class LocalizationExtensions
{
    public static IServiceCollection AddMyLocalization(this IServiceCollection services)
    {
        services.AddLocalization(x => x.ResourcesPath = LocalizationConsts.ResourcesFolder);

        return services;
    }

    public static WebApplication UseMyLocalization(this WebApplication app)
    {
        var supportedCultures = LocalizationConsts.SupportedCultures.Select(x => x.CultureInfo.Name)
            .ToArray();
        
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(LocalizationConsts.DefaultCulture)
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        return app;
    }
}