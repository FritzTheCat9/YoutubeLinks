using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Exceptions;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError("[Api Exception] {Exception}", exception);
            await HandleExceptionAsync(exception, context);
        }
    }

    private static async Task HandleExceptionAsync(
        Exception exception,
        HttpContext context)
    {
        var errorHelperModel = MapExceptionToResponse(exception);

        context.Response.StatusCode = errorHelperModel.StatusCode;
        context.Response.ContentType = "application/json";

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            }
        };

        var jsonString = JsonConvert.SerializeObject(errorHelperModel.ErrorResponse, settings);
        await context.Response.WriteAsync(jsonString);
    }

    private static ErrorHelperModel MapExceptionToResponse(Exception exception)
        => exception switch
        {
            MyValidationException validationException =>
                new ErrorHelperModel(StatusCodes.Status400BadRequest, validationException.ToErrorResponse()),
            MyServerException serverException =>
                new ErrorHelperModel(StatusCodes.Status500InternalServerError, serverException.ToErrorResponse()),
            MyUnauthorizedException unauthorizedException =>
                new ErrorHelperModel(StatusCodes.Status401Unauthorized, unauthorizedException.ToErrorResponse()),
            MyForbiddenException forbiddenException =>
                new ErrorHelperModel(StatusCodes.Status403Forbidden, forbiddenException.ToErrorResponse()),
            MyNotFoundException notFoundException =>
                new ErrorHelperModel(StatusCodes.Status404NotFound, notFoundException.ToErrorResponse()),
            _ =>
                new ErrorHelperModel(StatusCodes.Status500InternalServerError, CreateDefaultServerErrorResponse()),
        };

    private static ServerErrorResponse CreateDefaultServerErrorResponse()
        => new(ExceptionType.Server,
            "Internal server error: An unexpected issue occurred while processing your request.");

    private class ErrorHelperModel(int statusCode, ErrorResponse errorResponse)
    {
        public int StatusCode { get; } = statusCode;
        public ErrorResponse ErrorResponse { get; } = errorResponse;
    }
}