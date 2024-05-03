using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Pages.Users
{
    public partial class Auth : ComponentBase
    {
        [Parameter] public EventCallback<ThemeColor> ChangeThemeColor { get; set; }
        [Parameter] public EventCallback UserChanged { get; set; }

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IUserApiClient UserApiClient { get; set; }
        [Inject] public IAuthService AuthService { get; set; }

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public IJwtProvider JwtProvider { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var jwt = await JwtProvider.GetJwtDto();
                if (jwt == null || string.IsNullOrEmpty(jwt.RefreshToken))
                    await Logout();

                var newJwt = await UserApiClient.RefreshToken(new() { RefreshToken = jwt.RefreshToken });
                await JwtProvider.SetJwtDto(newJwt);

                var authStateProvider = AuthenticationStateProvider as AuthStateProvider;
                authStateProvider.NotifyAuthStateChanged();
            }
            catch (Exception)
            {
                await Logout();
            }
        }

        private async Task Login()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<LoginDialog>
            {
                {
                    x => x.Command,
                    new()
                }
            };

            var dialog = await DialogService.ShowAsync<LoginDialog>(Localizer[nameof(AppStrings.Login)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                if (result.Data is JwtDto token)
                {
                    await JwtProvider.SetJwtDto(token);

                    var authStateProvider = (AuthenticationStateProvider as AuthStateProvider);
                    authStateProvider.NotifyAuthStateChanged();

                    await LoadUserTheme();
                    await UserChanged.InvokeAsync();
                }
            }
        }

        private async Task LoadUserTheme()
        {
            var userId = await AuthService.GetCurrentUserId();
            if (userId is null)
                return;

            try
            {
                var user = await UserApiClient.GetUser(userId.Value);

                await ChangeThemeColor.InvokeAsync(user.ThemeColor);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
        }

        private async Task Register()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<RegisterDialog>
            {
                {
                    x => x.Command,
                    new()
                }
            };

            var dialog = await DialogService.ShowAsync<RegisterDialog>(Localizer[nameof(AppStrings.Register)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await OpenRegistrationSuccessDialog();
        }

        private async Task OpenRegistrationSuccessDialog()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<SuccessDialog>
            {
                {
                    x => x.ContentText,
                    Localizer[nameof(AppStrings.AccountCreated)]
                }
            };

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