using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users;

public partial class ResetPasswordPage
{
    private readonly ResetPassword.Command _command = new();
    private CustomValidator _customValidator;
    private bool _parsingError;
    private FritzProcessingButton _processingButton;
    private bool _success;

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

            _success = await UserApiClient.ResetPassword(_command);
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