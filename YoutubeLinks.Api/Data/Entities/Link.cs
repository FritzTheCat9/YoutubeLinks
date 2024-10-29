namespace YoutubeLinks.Api.Data.Entities;

public class Link : Entity
{
    public string Url { get; set; }
    public string VideoId { get; set; }
    public string Title { get; set; }
    public bool Downloaded { get; set; }

    public int PlaylistId { get; init; }
    public Playlist Playlist { get; set; }
}