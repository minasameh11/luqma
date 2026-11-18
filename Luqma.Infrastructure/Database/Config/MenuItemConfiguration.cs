using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItems");
            builder.HasKey(i => i.ID);

            builder.Property(i => i.NameAr).IsRequired().HasMaxLength(150);
            builder.Property(i => i.NameEn).IsRequired().HasMaxLength(150);
            builder.Property(i => i.DescriptionAr).HasMaxLength(1000);
            builder.Property(i => i.DescriptionEn).HasMaxLength(1000);
            builder.Property(i => i.ImageUrl).HasMaxLength(500);

            builder.Property(i => i.Price).HasPrecision(18, 2);
            builder.Property(i => i.CompareAtPrice).HasPrecision(18, 2);
            builder.Property(i => i.WeightMin).HasPrecision(18, 2);
            builder.Property(i => i.WeightMax).HasPrecision(18, 2);
            builder.Property(i => i.WeightStep).HasPrecision(18, 2);

            builder.HasOne(i => i.MenuCategory)
                   .WithMany(c => c.Items)
                   .HasForeignKey(i => i.MenuCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Items are filtered by category and by active state, and sorted by price
            builder.HasIndex(i => new { i.MenuCategoryId, i.IsArchived, i.IsActive });
            builder.HasIndex(i => new { i.IsArchived, i.IsActive, i.Price });
        }
    }
}
