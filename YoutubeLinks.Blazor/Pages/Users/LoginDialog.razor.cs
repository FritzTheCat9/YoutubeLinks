using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Shared.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users;

public partial class LoginDialog(
    IExceptionHandler exceptionHandler,
    IUserApiClient userApiClient,
    IStringLocalizer<App> localizer,
    IDialogService dialogService)
    : ComponentBase
{
    private CustomValidator _customValidator;
    private FritzProcessingButton _processingButton;

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
    [Parameter] public Login.Command Command { get; set; } = new();

    private async Task HandleValidSubmit()
    {
        try
        {
            _processingButton.SetProcessing(true);

            var token = await userApiClient.Login(Command);

            MudDialog.Close(DialogResult.Ok(token));
        }
        catch (MyValidationException validationException)
        {
            _customValidator.DisplayErrors(validationException.Errors);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
        finally
        {
            _processingButton.SetProcessing(false);
        }
    }

    private async Task OpenResendConfirmationEmailDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<ResendConfirmationEmailDialog>
        {
            {
                x => x.Command,
                new ResendConfirmationEmail.Command()
            }
        };

        var dialog =
            await dialogService.ShowAsync<ResendConfirmationEmailDialog>(
                localizer[nameof(AppStrings.ResendConfirmationEmail)], parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await OpenResendConfirmationEmailSuccessDialog();
        }
    }

    private async Task OpenResendConfirmationEmailSuccessDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<SuccessDialog>
        {
            {
                x => x.ContentText,
                localizer[nameof(AppStrings.ResendConfirmationEmailSent)]
            }
        };

        await dialogService.ShowAsync<SuccessDialog>(localizer[nameof(AppStrings.Success)], parameters, options);
    }

    private async Task OpenForgotPasswordDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<ForgotPasswordDialog>
        {
            {
                x => x.Command,
                new ForgotPassword.Command()
            }
        };

        var dialog =
            await dialogService.ShowAsync<ForgotPasswordDialog>(localizer[nameof(AppStrings.ForgotPassword)],
                parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await OpenForgotPasswordEmailSentSuccessfullyDialog();
        }
    }

    private async Task OpenForgotPasswordEmailSentSuccessfullyDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<SuccessDialog>
        {
            {
                x => x.ContentText,
                localizer[nameof(AppStrings.ForgotPasswordEmailSent)]
            }
        };

        await dialogService.ShowAsync<SuccessDialog>(localizer[nameof(AppStrings.Success)], parameters, options);
    }

    public abstract class LoginDialogConst
    {
        public const string EmailInput = "login-dialog-email-input";
        public const string PasswordInput = "login-dialog-password-input";
        public const string LoginButton = "login-dialog-login-button";
    }
}