using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

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
                _logger.LogError("[Api Exception] {Exception}", exception);
                await HandleExceptionAsync(exception, context);
            }
        }

        private static async Task HandleExceptionAsync(
            Exception exception,
            HttpContext context)
        {
            var errorHelperModel = exception switch
            {
                MyValidationException validationException =>
                    new ErrorHelperModel(
                        StatusCodes.Status400BadRequest,
                        new ValidationErrorResponse(validationException.Type, validationException.Message, validationException.Errors)),
                MyServerException serverException =>
                    new ErrorHelperModel(
                        StatusCodes.Status500InternalServerError,
                        new ServerErrorResponse(serverException.Type, serverException.Message)),
                MyUnauthorizedException unauthorizedException =>
                    new ErrorHelperModel(
                        StatusCodes.Status401Unauthorized,
                        new UnauthorizedErrorResponse(unauthorizedException.Type, unauthorizedException.Message)),
                MyForbiddenException forbiddenException =>
                    new ErrorHelperModel(
                        StatusCodes.Status403Forbidden,
                        new ForbiddenErrorResponse(forbiddenException.Type, forbiddenException.Message)),
                MyNotFoundException notFoundException =>
                    new ErrorHelperModel(
                        StatusCodes.Status404NotFound,
                        new NotFoundErrorResponse(notFoundException.Type, notFoundException.Message)),
                _ =>
                    new ErrorHelperModel(
                        StatusCodes.Status500InternalServerError,
                        new ServerErrorResponse(ExceptionType.Server, "Server Error")),
            };

            context.Response.StatusCode = errorHelperModel.StatusCode;
            context.Response.ContentType = "application/json";

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false,
                    }
                }
            };

            var jsonString = JsonConvert.SerializeObject(errorHelperModel.ErrorResponse, settings);
            await context.Response.WriteAsync(jsonString);
        }

        private class ErrorHelperModel
        {
            public int StatusCode { get; set; }
            public ErrorResponse ErrorResponse { get; set; }

            public ErrorHelperModel(int statusCode, ErrorResponse errorResponse)
            {
                StatusCode = statusCode;
                ErrorResponse = errorResponse;
            }
        }
    }
}
