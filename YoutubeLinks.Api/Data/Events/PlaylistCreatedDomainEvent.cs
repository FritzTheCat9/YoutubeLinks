namespace YoutubeLinks.Api.Data.Events
{
    public record PlaylistCreatedDomainEvent(int PlaylistId, int UserId) : IDomainEvent;
}
