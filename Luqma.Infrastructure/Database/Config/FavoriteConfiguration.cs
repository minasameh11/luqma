using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.ToTable("Favorites");
            builder.HasKey(f => f.ID);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.MenuItem)
                   .WithMany()
                   .HasForeignKey(f => f.MenuItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            // The favorites list is always read by user, newest first
            builder.HasIndex(f => new { f.UserId, f.IsArchived });

            // The same menu item cannot be saved twice in the active favorites
            builder.HasIndex(f => new { f.UserId, f.MenuItemId })
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");
        }
    }
}
