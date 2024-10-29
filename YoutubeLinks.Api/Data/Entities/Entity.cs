namespace YoutubeLinks.Api.Data.Entities;

public abstract class Entity
{
    public int Id { get; init; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}