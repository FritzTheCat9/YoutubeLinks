using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Blazor.Pages.Users;

public partial class ForgotPasswordDialog(
    IExceptionHandler exceptionHandler,
    IUserApiClient userApiClient,
    IStringLocalizer<App> localizer,
    IDialogService dialogService)
    : ComponentBase
{
    private CustomValidator _customValidator;
    private FritzProcessingButton _processingButton;

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
    [Parameter] public ForgotPassword.Command Command { get; set; } = new();

    private async Task HandleValidSubmit()
    {
        try
        {
            _processingButton.SetProcessing(true);

            await userApiClient.ForgotPassword(Command);

            MudDialog.Close(DialogResult.Ok(true));
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