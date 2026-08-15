using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Configurations;

public class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.ToTable("Links");
        builder.HasKey(l => l.Id);

        // Value Object (Url)
        builder.OwnsOne(l => l.Url, url =>
        {
            url.Property(p => p.Url)
                .HasColumnName("Url")
                .HasMaxLength(500)
                .IsRequired();

            url.Property(p => p.VideoId)
                .HasColumnName("VideoId")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(l => l.Title, t =>
        {
            t.Property(p => p.Value)
             .HasColumnName("Title")
             .HasMaxLength(300)
             .IsRequired();
        });

        builder.Property(l => l.Downloaded)
               .IsRequired();

        // Relations
        builder.HasOne(l => l.Playlist)
               .WithMany(p => p.Links)
               .HasForeignKey(l => l.PlaylistId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}