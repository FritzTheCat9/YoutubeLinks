using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users
{
    public partial class ConfirmEmailPage : ComponentBase
    {
        private CustomValidator _customValidator;
        private FritzProcessingButton _processingButton;

        private readonly ConfirmEmail.Command _command = new();

        private bool _success;
        private bool _parsingError;

        [SupplyParameterFromQuery] public string Email { get; set; }
        [SupplyParameterFromQuery] public string Token { get; set; }

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IUserApiClient UserApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        protected override void OnParametersSet()
        {
            try
            {
                _command.Email = Uri.UnescapeDataString(Email);
                _command.Token = Uri.UnescapeDataString(Token);
            }
            catch (Exception)
            {
                _parsingError = true;
            }
        }

        private async Task OnValidSubmit()
        {
            try
            {
                _processingButton.SetProcessing(true);

                _success = await UserApiClient.ConfirmEmail(_command);
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