using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Pages.Error;

public partial class ServerErrorPage(
    IStringLocalizer<App> localizer)
    : ComponentBase { }