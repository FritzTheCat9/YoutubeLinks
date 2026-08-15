namespace YoutubeLinks.Api.Data.ValueObjects;

public sealed class PlaylistName : ValueObject
{
    public string Value { get; private set; }

    private PlaylistName() { }

    public PlaylistName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Playlist name is required.");

        if (value.Length > 100)
            throw new ArgumentException("Playlist name too long.");

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PlaylistName name) => name?.Value;

    public static implicit operator PlaylistName(string value) => value is null ? null : new PlaylistName(value);
}