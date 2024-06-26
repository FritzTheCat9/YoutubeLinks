using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Blazor.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users
{
    public partial class RegisterDialog : ComponentBase
    {
        private CustomValidator _customValidator;
        private FritzProcessingButton _processingButton;

        public class RegisterDialogConst
        {
            public const string EmailInput = "register-dialog-email-input";
            public const string UserNameInput = "register-dialog-username-input";
            public const string PasswordInput = "register-dialog-password-input";
            public const string RepeatPasswordInput = "register-dialog-repeat-password-input";
            public const string RegisterButton = "register-dialog-register-button";
        }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        [Parameter] public Register.Command Command { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IUserApiClient UserApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IThemeColorProvider ThemeColorProvider { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
                _processingButton.SetProcessing(true);

                Command.ThemeColor = await ThemeColorProvider.GetThemeColor();

                await UserApiClient.Register(Command);

                MudDialog.Close(DialogResult.Ok(true));
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
    }
}