using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace YoutubeLinks.Blazor.Pages;

public partial class HomePage(
    IStringLocalizer<App> localizer)
    : ComponentBase { }