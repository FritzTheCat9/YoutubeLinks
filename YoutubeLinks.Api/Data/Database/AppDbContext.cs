using Microsoft.EntityFrameworkCore;
using System.Reflection;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Database
{
    public class AppDbContext : DbContext
    {
        private readonly IClock _clock;

        public DbSet<User> Users { get; set; }

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IClock clock) : base(options)
        {
            _clock = clock;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var _date = _clock.Current();

            modelBuilder.Entity<User>().HasData(new List<User>()
            {
                new() { Id = 1, Email = "freakfightsfan@gmail.com", UserName = "Admin", Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==", EmailConfirmed = true, EmailConfirmationToken = null, IsAdmin = true, Created = _date, Modified = _date },
                new() { Id = 2, Email = "freakfightsfan1@gmail.com", UserName = "User", Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==", EmailConfirmed = true, EmailConfirmationToken = null, IsAdmin = false, Created = _date, Modified = _date },
            });
        }
    }
}
