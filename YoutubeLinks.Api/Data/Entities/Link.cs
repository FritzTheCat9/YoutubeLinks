using YoutubeLinks.Api.Data.ValueObjects;

namespace YoutubeLinks.Api.Data.Entities;

public class Link : Entity
{
    public int PlaylistId { get; private set; }
    public Playlist Playlist { get; private set; }

    public YoutubeUrl Url { get; private set; }
    public Title Title { get; private set; }
    public string VideoId => Url?.VideoId;
    public bool Downloaded { get; private set; }

    private Link() { }

    public static Link Create(YoutubeUrl url, string title, Playlist playlist)
    {
        return new Link
        {
            Url = new YoutubeUrl(url),
            Title = string.IsNullOrWhiteSpace(title) ? null : new Title(title),
            Downloaded = false,
            Playlist = playlist
        };
    }

    public static Link Create(string url, string title, Playlist playlist)
    {
        // accept the old signature for compatibility; YoutubeUrl will validate/extract id
        var youtubeUrl = new YoutubeUrl(url);
        return Create(youtubeUrl, title, playlist);
    }

    public void SetTitle(string title)
    {
        Title = new Title(title);
        UpdateModified();
    }

    public void SetUrl(string url)
    {
        Url = new YoutubeUrl(url);
        UpdateModified();
    }

    public void SetDownloaded(bool downloaded)
    {
        Downloaded = downloaded;
        UpdateModified();
    }
}
