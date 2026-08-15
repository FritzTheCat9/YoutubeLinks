namespace YoutubeLinks.Api.Data.ValueObjects;

public sealed class YoutubeUrl : ValueObject
{
    public string Url { get; private set; }
    public string VideoId { get; private set; }

    private YoutubeUrl() { }

    public YoutubeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url cannot be empty.");

        VideoId = ExtractVideoId(url)
                  ?? throw new ArgumentException("Invalid YouTube URL.");

        Url = $"https://www.youtube.com/watch?v={VideoId}";
    }

    public static string ExtractVideoId(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        // Support multiple YouTube URL formats: v= query, youtu.be short links, and embed URLs
        try
        {
            var m = System.Text.RegularExpressions.Regex.Match(url, @"(?:v=|youtu\.be/|embed/)([A-Za-z0-9_-]+)");
            if (m.Success && m.Groups.Count > 1)
                return m.Groups[1].Value;
        }
        catch
        {
            // ignore regex errors and fall through to null
        }

        return null;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Url;
        yield return VideoId;
    }

    public override string ToString() => Url;

    public static implicit operator string(YoutubeUrl u) => u?.Url;

    public static implicit operator YoutubeUrl(string value) => value is null ? null : new YoutubeUrl(value);
}