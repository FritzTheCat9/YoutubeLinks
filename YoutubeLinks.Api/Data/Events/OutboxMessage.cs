namespace YoutubeLinks.Api.Data.Events;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = "";
    public string Data { get; set; } = "";
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
}
