using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Configurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.ToTable("Playlists");
        builder.HasKey(p => p.Id);

        // Value Object (Name)
        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(p => p.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(p => p.IsPublic)
               .IsRequired();


        // Relations
        builder.HasOne(p => p.User)
               .WithMany(u => u.Playlists)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Links)
               .WithOne(l => l.Playlist)
               .HasForeignKey(l => l.PlaylistId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}