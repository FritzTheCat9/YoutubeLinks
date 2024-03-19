using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Exceptions;

namespace YoutubeLinks.Blazor.Pages.Error
{
    public partial class ValidationErrorPage : ComponentBase
    {
        [Inject] public ValidationErrors ValidationErrors { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}