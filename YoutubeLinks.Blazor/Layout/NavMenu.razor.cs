using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Layout
{
    public partial class NavMenu
    {
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}