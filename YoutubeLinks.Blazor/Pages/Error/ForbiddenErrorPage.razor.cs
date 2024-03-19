using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Pages.Error
{
    public partial class ForbiddenErrorPage : ComponentBase
    {
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}