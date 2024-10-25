using Microsoft.Extensions.Configuration;

namespace YoutubeLinks.Shared.Extensions;

public static class OptionsExtensions
{
    public static T GetOptions<T>(
        this IConfiguration configuration,
        string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}