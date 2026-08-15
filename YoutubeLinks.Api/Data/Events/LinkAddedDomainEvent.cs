namespace YoutubeLinks.Api.Data.Events
{
    public record LinkAddedDomainEvent(int PlaylistId, int LinkId, string VideoId) : IDomainEvent;
}
