using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.Blazor.Pages.Links;

public partial class CreateLinkForm(
    IExceptionHandler exceptionHandler,
    ILinkApiClient linkApiClient,
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    private CustomValidator _customValidator;
    private FritzProcessingButton _processingButton;

    [Parameter] public CreateLink.Command Command { get; set; } = new();
    [Parameter] public EventCallback ParentReloadFunction { get; set; }

    private async Task HandleValidSubmit()
    {
        try
        {
            _processingButton.SetProcessing(true);

            await linkApiClient.CreateLink(Command);

            Command.Url = "";
            await ParentReloadFunction.InvokeAsync();
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

    public class CreateLinkFormConst
    {
        public const string UrlInput = "create-link-form-url-input";
        public const string CreateButton = "create-link-form-create-button";
    }
}