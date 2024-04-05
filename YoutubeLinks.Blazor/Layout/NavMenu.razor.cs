using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Auth;

namespace YoutubeLinks.Blazor.Layout
{
    public partial class NavMenu
    {
        public int? UserId { get; set; }

        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            UserId = await AuthService.GetCurrentUserId();
        }
    }
}