using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users
{
    public partial class LoginDialog : ComponentBase
    {
        private CustomValidator _customValidator;
        private FritzProcessingButton _processingButton;

        public class LoginDialogConst
        {
            public const string EmailInput = "login-dialog-email-input";
            public const string PasswordInput = "login-dialog-password-input";
            public const string LoginButton = "login-dialog-login-button";
        }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
        [Parameter] public Login.Command Command { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IUserApiClient UserApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IDialogService DialogService { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
                _processingButton.SetProcessing(true);

                var token = await UserApiClient.Login(Command);

                MudDialog.Close(DialogResult.Ok(token));
            }
            catch (MyValidationException validationException)
            {
                _customValidator.DisplayErrors(validationException.Errors);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
            finally
            {
                _processingButton.SetProcessing(false);
            }
        }

        private async Task OpenResendConfirmationEmailDialog()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<ResendConfirmationEmailDialog>
            {
                {
                    x => x.Command,
                    new()
                }
            };

            var dialog = await DialogService.ShowAsync<ResendConfirmationEmailDialog>(Localizer[nameof(AppStrings.ResendConfirmationEmail)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await OpenResendConfirmationEmailSuccessDialog();
        }

        private async Task OpenResendConfirmationEmailSuccessDialog()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<SuccessDialog>
            {
                {
                    x => x.ContentText,
                    Localizer[nameof(AppStrings.ResendConfirmationEmailSent)]
                }
            };

            var dialog = await DialogService.ShowAsync<SuccessDialog>(Localizer[nameof(AppStrings.Success)], parameters, options);
        }

        private async Task OpenForgotPasswordDialog()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<ForgotPasswordDialog>
            {
                {
                    x => x.Command,
                    new()
                }
            };

            var dialog = await DialogService.ShowAsync<ForgotPasswordDialog>(Localizer[nameof(AppStrings.ForgotPassword)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await OpenForgotPasswordEmailSentSuccessfullyDialog();
        }

        private async Task OpenForgotPasswordEmailSentSuccessfullyDialog()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<SuccessDialog>
            {
                {
                    x => x.ContentText,
                    Localizer[nameof(AppStrings.ForgotPasswordEmailSent)]
                }
            };

            var dialog = await DialogService.ShowAsync<SuccessDialog>(Localizer[nameof(AppStrings.Success)], parameters, options);
        }
    }
}