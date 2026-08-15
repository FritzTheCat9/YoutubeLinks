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
    var parts = url.Split(new[] { "v=" }, StringSplitOptions.None);
    return parts.Length > 1 ? parts[1].Split('&')[0] : null;
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