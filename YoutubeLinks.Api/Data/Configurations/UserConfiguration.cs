using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // Value Objects
        builder.OwnsOne(u => u.Email, e =>
        {
            e.Property(p => p.Value)
             .HasColumnName("Email")
             .HasMaxLength(200)
             .IsRequired();
            // Create unique index on the owned Email value
            e.HasIndex(p => p.Value).IsUnique();
        });

        builder.OwnsOne(u => u.UserName, un =>
        {
            un.Property(p => p.Value)
              .HasColumnName("UserName")
              .HasMaxLength(50)
              .IsRequired();
            // Create unique index on the owned UserName value
            un.HasIndex(p => p.Value).IsUnique();
        });


        // Relations
        builder.HasMany(u => u.Playlists)
               .WithOne(p => p.User)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}