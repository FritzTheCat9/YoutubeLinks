using YoutubeLinks.Api.Data.Events;

namespace YoutubeLinks.Api.Data.Entities;

public abstract class Entity
{
    public int Id { get; init; }
    public DateTime Created { get; private set; } = DateTime.UtcNow;
    public DateTime Modified { get; private set; } = DateTime.UtcNow;

    private readonly List<IDomainEvent> _events = [];
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);

    protected void UpdateModified() => Modified = DateTime.UtcNow;
    public void ClearEvents() => _events.Clear();
}