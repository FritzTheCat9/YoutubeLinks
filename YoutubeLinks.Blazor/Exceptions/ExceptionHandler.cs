using Microsoft.AspNetCore.Components;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Blazor.Exceptions;

public interface IExceptionHandler
{
    void HandleExceptions(Exception exception);
}

public class ExceptionHandler : IExceptionHandler
{
    private readonly NavigationManager _navigationManager;
    private readonly ValidationErrors _validationErrors;

    public ExceptionHandler(
        NavigationManager navigationManager,
        ValidationErrors validationErrors)
    {
        _navigationManager = navigationManager;
        _validationErrors = validationErrors;
    }

    public void HandleExceptions(Exception exception)
    {
        switch (exception)
        {
            case MyUnauthorizedException:
                _navigationManager.NavigateTo("error/unauthorized-error");
                break;
            case MyForbiddenException:
                _navigationManager.NavigateTo("error/forbidden-error");
                break;
            case MyNotFoundException:
                _navigationManager.NavigateTo("error/notfound-error");
                break;
            case MyValidationException validationException:
                _validationErrors.Errors = validationException.Errors;
                _navigationManager.NavigateTo("error/validation-error");
                break;
            case MyServerException:
                _navigationManager.NavigateTo("error/server-error");
                break;
            default:
                //TODO: log other HttpService exceptions to file on the server
                _navigationManager.NavigateTo("error/server-error");
                break;
        }
    }
}