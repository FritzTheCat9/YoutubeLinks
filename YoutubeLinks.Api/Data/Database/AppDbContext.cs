using System.Reflection;
using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Database;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Link> Links { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<User>().HasData(new List<User>
        {
            new()
            {
                Id = 1, Email = "ytlinksapp@gmail.com", UserName = "Admin",
                Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==",
                EmailConfirmed = true, EmailConfirmationToken = null, IsAdmin = true,
                Created = new DateTime(2024, 12, 12),
                Modified = new DateTime(2024, 12, 12)
            },
            new()
            {
                Id = 2, Email = "ytlinksapp1@gmail.com", UserName = "User",
                Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==",
                EmailConfirmed = true, EmailConfirmationToken = null, IsAdmin = false,
                Created = new DateTime(2024, 12, 12),
                Modified = new DateTime(2024, 12, 12)
            }
        });
    }
}