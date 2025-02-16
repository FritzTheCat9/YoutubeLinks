namespace YoutubeLinks.Api.Data.Entities;

public class Link : Entity
{
    public int PlaylistId { get; private set; }
    public Playlist Playlist { get; private set; }

    public string Url { get; private set; }
    public string VideoId { get; private set; }
    public string Title { get; private set; }
    public bool Downloaded { get; private set; }

    private Link() { }

    public static Link Create(string url, string videoId, string title, Playlist playlist)
    {
        return new Link
        {
            Url = url,
            VideoId = videoId,
            Title = title,
            Downloaded = false,
            Playlist = playlist
        };
    }

    public void SetTitle(string title)
    {
        Title = title;
        UpdateModified();
    }

    public void SetUrl(string url)
    {
        Url = url;
        UpdateModified();
    }

    public void SetVideoId(string videoId)
    {
        VideoId = videoId;
        UpdateModified();
    }

    public void SetDownloaded(bool downloaded)
    {
        Downloaded = downloaded;
        UpdateModified();
    }
}