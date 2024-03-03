namespace YoutubeLinks.Blazor.Exceptions
{
    public class ValidationErrors
    {
        public Dictionary<string, List<string>> Errors { get; set; } = [];
    }
}
