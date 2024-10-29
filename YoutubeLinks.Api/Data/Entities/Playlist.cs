namespace YoutubeLinks.Api.Data.Entities;

public class Playlist : Entity
{
    public string Name { get; set; }
    public bool Public { get; set; }

    public int UserId { get; init; }
    public User User { get; set; }
    public List<Link> Links { get; } = [];
}