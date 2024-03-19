using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Pages
{
    public partial class HomePage : ComponentBase
    {
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}