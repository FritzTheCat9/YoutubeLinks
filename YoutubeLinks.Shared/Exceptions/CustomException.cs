namespace YoutubeLinks.Shared.Exceptions;

public abstract class CustomException(string message) : Exception(message)
{
    public ExceptionType Type { get; protected init; }
    public abstract ErrorResponse ToErrorResponse();
}

public class MyValidationException : CustomException
{
    private static readonly string _message =
        "Bad request: One or more validation errors were found in your submission.";

    public Dictionary<string, List<string>> Errors { get; private set; } = [];

    public MyValidationException() : base(_message)
    {
        Type = ExceptionType.Validation;
    }

    public MyValidationException(Dictionary<string, List<string>> errors) : base(_message)
    {
        Type = ExceptionType.Validation;

        Errors = errors ?? [];
    }

    public MyValidationException(string propertyName, string error) : base(_message)
    {
        Type = ExceptionType.Validation;

        if (!Errors.TryGetValue(propertyName, out var value))
        {
            value = [];
            Errors[propertyName] = value;
        }

        value.Add(error);
    }

    public override ErrorResponse ToErrorResponse() => new ValidationErrorResponse(Type, Message, Errors);
}

public class MyServerException : CustomException
{
    private static readonly string _message =
        "Internal server error: An unexpected issue occurred while processing your request.";

    public MyServerException(string message) : base(message)
    {
        Type = ExceptionType.Server;
    }

    public MyServerException() : base(_message)
    {
        Type = ExceptionType.Server;
    }

    public override ErrorResponse ToErrorResponse() => new ServerErrorResponse(Type, Message);
}

public class MyUnauthorizedException : CustomException
{
    private static readonly string _message =
        "Unauthorized access: You do not have the necessary credentials to perform this action.";

    public MyUnauthorizedException() : base(_message)
    {
        Type = ExceptionType.Unauthorized;
    }

    public override ErrorResponse ToErrorResponse() => new UnauthorizedErrorResponse(Type, Message);
}

public class MyForbiddenException : CustomException
{
    private static readonly string _message = "Access denied: You do not have permission to view this resource.";

    public MyForbiddenException() : base(_message)
    {
        Type = ExceptionType.Forbidden;
    }

    public override ErrorResponse ToErrorResponse() => new ForbiddenErrorResponse(Type, Message);
}

public class MyNotFoundException : CustomException
{
    private static readonly string _message =
        "Resource not found: The requested item could not be located on the server.";

    public MyNotFoundException() : base(_message)
    {
        Type = ExceptionType.NotFound;
    }

    public override ErrorResponse ToErrorResponse() => new NotFoundErrorResponse(Type, Message);
}