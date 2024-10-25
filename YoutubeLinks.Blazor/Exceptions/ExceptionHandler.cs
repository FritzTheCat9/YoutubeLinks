using Microsoft.AspNetCore.Components;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Blazor.Exceptions;

public interface IExceptionHandler
{
    void HandleExceptions(Exception exception);
}

public class ExceptionHandler(
    NavigationManager navigationManager,
    ValidationErrors validationErrors)
    : IExceptionHandler
{
    public void HandleExceptions(Exception exception)
    {
        switch (exception)
        {
            case MyUnauthorizedException:
                navigationManager.NavigateTo("error/unauthorized-error");
                break;
            case MyForbiddenException:
                navigationManager.NavigateTo("error/forbidden-error");
                break;
            case MyNotFoundException:
                navigationManager.NavigateTo("error/notfound-error");
                break;
            case MyValidationException validationException:
                validationErrors.Errors = validationException.Errors;
                navigationManager.NavigateTo("error/validation-error");
                break;
            case MyServerException:
                navigationManager.NavigateTo("error/server-error");
                break;
            default:
                //TODO: log other HttpService exceptions to file on the server
                navigationManager.NavigateTo("error/server-error");
                break;
        }
    }
}