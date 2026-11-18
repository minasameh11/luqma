using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");
            builder.HasKey(c => c.ID);

            builder.Property(c => c.ConfigurationSignature).IsRequired().HasMaxLength(600);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.MenuItem)
                   .WithMany()
                   .HasForeignKey(c => c.MenuItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.MenuItemOption)
                   .WithMany()
                   .HasForeignKey(c => c.MenuItemOptionId)
                   .OnDelete(DeleteBehavior.Restrict);

            // The cart is always read by user; a configuration appears once per active cart
            builder.HasIndex(c => new { c.UserId, c.IsArchived });
            builder.HasIndex(c => new { c.UserId, c.MenuItemId, c.ConfigurationSignature })
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");
        }
    }
}
