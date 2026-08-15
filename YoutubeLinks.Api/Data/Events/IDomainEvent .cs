namespace YoutubeLinks.Api.Data.Events;

public interface IDomainEvent
{

}

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}