namespace YoutubeLinks.Shared.Exceptions
{
    public class ErrorResponse
    {
        public ExceptionType Type { get; set; }
        public string Message { get; set; }

        public ErrorResponse(ExceptionType type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    public class ValidationErrorResponse : ErrorResponse
    {
        public Dictionary<string, List<string>> Errors { get; set; } = [];

        public ValidationErrorResponse(
            ExceptionType type,
            string message,
            Dictionary<string, List<string>> errors)
            : base(type, message)
        {
            Errors = errors;
        }
    }

    public class ServerErrorResponse : ErrorResponse
    {
        public ServerErrorResponse(ExceptionType type, string message) : base(type, message) { }
    }

    public class UnauthorizedErrorResponse : ErrorResponse
    {
        public UnauthorizedErrorResponse(ExceptionType type, string message) : base(type, message) { }
    }

    public class ForbiddenErrorResponse : ErrorResponse
    {
        public ForbiddenErrorResponse(ExceptionType type, string message) : base(type, message) { }
    }

    public class NotFoundErrorResponse : ErrorResponse
    {
        public NotFoundErrorResponse(ExceptionType type, string message) : base(type, message) { }
    }
}
