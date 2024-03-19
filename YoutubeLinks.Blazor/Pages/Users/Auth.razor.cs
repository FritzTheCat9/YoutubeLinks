using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Pages.Users
{
    public partial class Auth : ComponentBase
    {
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public IJwtProvider JwtProvider { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IDialogService DialogService { get; set; }

        private async Task Login()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<LoginDialog>();
            parameters.Add(x => x.Command, new());

            var dialog = await DialogService.ShowAsync<LoginDialog>(Localizer[nameof(AppStrings.Login)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                if (result.Data is JwtDto token)
                {
                    await JwtProvider.SetJwtDto(token);

                    var authStateProvider = (AuthenticationStateProvider as AuthStateProvider);
                    authStateProvider.NotifyAuthStateChanged();
                }
            }
        }

        private async Task Register()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<RegisterDialog>();
            parameters.Add(x => x.Command, new());

            var dialog = await DialogService.ShowAsync<RegisterDialog>(Localizer[nameof(AppStrings.Register)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await OpenRegistrationSuccessDialog();
        }

        private async Task OpenRegistrationSuccessDialog()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<SuccessDialog>();
            parameters.Add(x => x.ContentText, Localizer[nameof(AppStrings.AccountCreated)]);

            var dialog = await DialogService.ShowAsync<SuccessDialog>(Localizer[nameof(AppStrings.Success)], parameters, options);
        }

        private async Task Logout()
        {
            await JwtProvider.RemoveJwtDto();

            var authStateProvider = (AuthenticationStateProvider as AuthStateProvider);
            authStateProvider.NotifyAuthStateChanged();

            NavigationManager.NavigateTo("/");
        }
    }
}