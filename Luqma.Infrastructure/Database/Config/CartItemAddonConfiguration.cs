using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class CartItemAddonConfiguration : IEntityTypeConfiguration<CartItemAddon>
    {
        public void Configure(EntityTypeBuilder<CartItemAddon> builder)
        {
            builder.ToTable("CartItemAddons");
            builder.HasKey(a => a.ID);

            builder.HasOne(a => a.CartItem)
                   .WithMany(c => c.Addons)
                   .HasForeignKey(a => a.CartItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.MenuItemAddon)
                   .WithMany()
                   .HasForeignKey(a => a.MenuItemAddonId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Addons are always loaded with their line; the same addon cannot repeat on a line
            builder.HasIndex(a => new { a.CartItemId, a.MenuItemAddonId })
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");
        }
    }
}
