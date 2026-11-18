using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class MenuItemOptionConfiguration : IEntityTypeConfiguration<MenuItemOption>
    {
        public void Configure(EntityTypeBuilder<MenuItemOption> builder)
        {
            builder.ToTable("MenuItemOptions");
            builder.HasKey(o => o.ID);

            builder.Property(o => o.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(o => o.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(o => o.Price).HasPrecision(18, 2);

            builder.HasOne(o => o.MenuItem)
                   .WithMany(i => i.Options)
                   .HasForeignKey(o => o.MenuItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => new { o.MenuItemId, o.IsArchived });
        }
    }

    public class MenuItemAddonConfiguration : IEntityTypeConfiguration<MenuItemAddon>
    {
        public void Configure(EntityTypeBuilder<MenuItemAddon> builder)
        {
            builder.ToTable("MenuItemAddons");
            builder.HasKey(a => a.ID);

            builder.Property(a => a.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(a => a.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Price).HasPrecision(18, 2);
            builder.Property(a => a.ImageUrl).HasMaxLength(500);

            builder.HasOne(a => a.MenuItem)
                   .WithMany(i => i.Addons)
                   .HasForeignKey(a => a.MenuItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => new { a.MenuItemId, a.IsArchived });
        }
    }
}
