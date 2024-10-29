using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Pages.Error;

public partial class UnauthorizedErrorPage(
    IStringLocalizer<App> localizer)
    : ComponentBase { }