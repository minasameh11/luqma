using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
    {
        public void Configure(EntityTypeBuilder<MenuCategory> builder)
        {
            builder.ToTable("MenuCategories");
            builder.HasKey(c => c.ID);

            builder.Property(c => c.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(c => c.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ImageUrl).HasMaxLength(500);

            builder.HasOne(c => c.Store)
                   .WithMany()
                   .HasForeignKey(c => c.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);

            // The menu is always read per store, ordered for display
            builder.HasIndex(c => new { c.StoreId, c.IsArchived, c.DisplayOrder });
        }
    }
}
