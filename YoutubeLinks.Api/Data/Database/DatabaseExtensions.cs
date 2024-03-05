using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Data.Database
{
    public static class DatabaseExtensions
    {
        private const string _sectionName = "Database";

        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(configuration.GetRequiredSection(_sectionName));
            var options = configuration.GetOptions<DatabaseOptions>(_sectionName);

            services.AddSingleton<IClock, Clock>();

            services.AddDbContext<AppDbContext>(x =>
            {
                x.UseSqlServer(options.ConnectionString);
                //x.EnableSensitiveDataLogging();
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<ILinkRepository, LinkRepository>();

            return services;
        }
    }
}
