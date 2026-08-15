using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.UnitTests.Builders;

public class LinkBuilder
{
    private string _url = "https://youtu.be/test";
    private string _title = "Sample Title";
    private Playlist _playlist;

    public LinkBuilder WithUrl(string url) { _url = url; return this; }
    public LinkBuilder WithTitle(string title) { _title = title; return this; }
    public LinkBuilder InPlaylist(Playlist playlist) { _playlist = playlist; return this; }

    public Link Build()
    {
        if (_playlist is null)
            throw new InvalidOperationException("Playlist is required. Use .InPlaylist(playlist)");

        return _playlist.AddLink(_url, _title);
    }

    public static LinkBuilder Create() => new();
}
