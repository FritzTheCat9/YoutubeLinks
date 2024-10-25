namespace YoutubeLinks.Blazor.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITableViewProvider, TableViewProvider>();
        services.AddScoped<IThemeColorProvider, ThemeColorProvider>();

        return services;
    }
}