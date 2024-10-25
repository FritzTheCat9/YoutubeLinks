using System.Reflection;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Database;

public class AppDbContext : DbContext
{
    private readonly IClock _clock;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IClock clock) : base(options)
    {
        _clock = clock;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Link> Links { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var date = _clock.Current();

        modelBuilder.Entity<User>().HasData(new List<User>
        {
            new()
            {
                Id = 1, Email = "ytlinksapp@gmail.com", UserName = "Admin",
                Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==",
                EmailConfirmed = true, EmailConfirmationToken = null, IsAdmin = true, Created = date, Modified = date
            },
            new()
            {
                Id = 2, Email = "ytlinksapp1@gmail.com", UserName = "User",
                Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==",
                EmailConfirmed = true, EmailConfirmationToken = null, IsAdmin = false, Created = date, Modified = date
            }
        });
    }
}