using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Pages.Users;

public partial class Auth(
    IExceptionHandler exceptionHandler,
    IUserApiClient userApiClient,
    IAuthService authService,
    IStringLocalizer<App> localizer,
    IDialogService dialogService)
    : ComponentBase
{
    [Parameter] public EventCallback<ThemeColor> ChangeThemeColor { get; set; }
    [Parameter] public EventCallback UserChanged { get; set; }

    private async Task Login()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<LoginDialog>
        {
            {
                x => x.Command,
                new Login.Command()
            }
        };

        var dialog =
            await dialogService.ShowAsync<LoginDialog>(localizer[nameof(AppStrings.Login)], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            if (result.Data is JwtDto token)
            {
                await authService.Login(token);

                await LoadUserTheme();
                await UserChanged.InvokeAsync();
            }
        }
    }

    private async Task LoadUserTheme()
    {
        var userId = await authService.GetCurrentUserId();
        if (userId is null)
        {
            return;
        }

        try
        {
            var user = await userApiClient.GetUser(userId.Value);

            await ChangeThemeColor.InvokeAsync(user.ThemeColor);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
    }

    private async Task Register()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<RegisterDialog>
        {
            {
                x => x.Command,
                new Register.Command()
            }
        };

        var dialog =
            await dialogService.ShowAsync<RegisterDialog>(localizer[nameof(AppStrings.Register)], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await OpenRegistrationSuccessDialog();
        }
    }

    private async Task OpenRegistrationSuccessDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<SuccessDialog>
        {
            {
                x => x.ContentText,
                localizer[nameof(AppStrings.AccountCreated)]
            }
        };

        await dialogService.ShowAsync<SuccessDialog>(localizer[nameof(AppStrings.Success)], parameters, options);
    }

    private async Task Logout()
    {
        await authService.Logout("/");
    }

    public class AuthComponentConst
    {
        public const string UserNameText = "auth-username-text";
        public const string LogoutButton = "auth-logout-button";
        public const string LoginButton = "auth-login-button";
        public const string RegisterButton = "auth-register-button";
    }
}