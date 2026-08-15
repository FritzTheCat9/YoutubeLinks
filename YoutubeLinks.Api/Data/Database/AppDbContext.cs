using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Events;

namespace YoutubeLinks.Api.Data.Database;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .SelectMany(x => x.Entity.Events)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            OutboxMessages.Add(new OutboxMessage
            {
                Type = domainEvent.GetType().Name,
                Data = JsonSerializer.Serialize(domainEvent),
                //OccurredOn = domainEvent.OccurredOn
            });
        }

        // Clear events to avoid duplicate publishing
        ChangeTracker.Entries<Entity>()
            .ToList()
            .ForEach(e => e.Entity.ClearEvents());

        return await base.SaveChangesAsync(ct);
    }
}