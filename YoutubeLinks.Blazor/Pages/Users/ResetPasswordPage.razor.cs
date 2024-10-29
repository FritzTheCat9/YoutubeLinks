using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users;

public partial class ResetPasswordPage(
    IExceptionHandler exceptionHandler,
    IUserApiClient userApiClient,
    IStringLocalizer<App> localizer)
{
    public ResetPassword.Command Command { get; set; } = new();
    private CustomValidator _customValidator;
    private bool _parsingError;
    private FritzProcessingButton _processingButton;
    private bool _success;

    [SupplyParameterFromQuery] public string Email { get; set; }
    [SupplyParameterFromQuery] public string Token { get; set; }

    protected override void OnParametersSet()
    {
        try
        {
            Command.Email = Uri.UnescapeDataString(Email);
            Command.Token = Uri.UnescapeDataString(Token);
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

            _success = await userApiClient.ResetPassword(Command);
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
}