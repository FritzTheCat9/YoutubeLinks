using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Pages.Error;

public partial class ForbiddenErrorPage(
    IStringLocalizer<App> localizer)
    : ComponentBase { }