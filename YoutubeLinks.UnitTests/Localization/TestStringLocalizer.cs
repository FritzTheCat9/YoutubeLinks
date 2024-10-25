using Microsoft.Extensions.Localization;
using System.Globalization;

namespace YoutubeLinks.UnitTests.Localization
{
    public class TestStringLocalizer<T> : IStringLocalizer<T>
    {
        private readonly Dictionary<string, string> _localizations;

        public TestStringLocalizer()
        {
            _localizations = new Dictionary<string, string>();
        }

        public LocalizedString this[string name] 
            => new(name, _localizations.TryGetValue(name, out var value) ? value : name, true);

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = _localizations.TryGetValue(name, out var localization) ? localization : name;
                var formatted = string.Format(CultureInfo.CurrentCulture, format, arguments);
                return new LocalizedString(name, formatted, true);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public void AddTranslation(string key, string value)
        {
            _localizations[key] = value;
        }
    }
}
