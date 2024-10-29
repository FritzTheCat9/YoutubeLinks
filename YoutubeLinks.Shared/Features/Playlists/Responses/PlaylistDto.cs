namespace YoutubeLinks.Shared.Features.Playlists.Responses;

public class PlaylistDto
{
    public int Id { get; init; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    public string Name { get; set; }
    public bool Public { get; set; }
    public int UserId { get; set; }
}