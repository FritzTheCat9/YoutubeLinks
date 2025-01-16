namespace YoutubeLinks.Api.Data.Entities;

public abstract class Entity
{
    public int Id { get; init; }
    public DateTime Created { get; private set; } = DateTime.UtcNow;
    public DateTime Modified { get; private set; } = DateTime.UtcNow;

    protected void UpdateModified() => Modified = DateTime.UtcNow;
}