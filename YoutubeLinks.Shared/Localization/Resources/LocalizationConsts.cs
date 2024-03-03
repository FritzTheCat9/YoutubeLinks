using System.Globalization;

namespace YoutubeLinks.Shared.Localization.Resources
{
    public static class LocalizationConsts
    {
        public static readonly string ResourcesFolder = "Localization/Resources";
        public static readonly string CultureKey = "Culture";
        public static readonly string DefaultCulture = "en";

        public static readonly Culture[] SupportedCultures = [
            new() { Country = "English", CultureInfo = new CultureInfo("en") },
        ];
    }

    public class Culture
    {
        public string Country { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
