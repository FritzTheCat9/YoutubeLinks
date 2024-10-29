using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Auth;

namespace YoutubeLinks.Blazor.Layout;

public partial class NavMenu(
    IAuthService authService,
    IStringLocalizer<App> localizer) : ComponentBase
{
    [Parameter] public int? UserId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        UserId = await authService.GetCurrentUserId();
    }
}