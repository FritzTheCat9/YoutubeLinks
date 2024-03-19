using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor
{
    public partial class App
    {
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}