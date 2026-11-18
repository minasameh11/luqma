using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores", t =>
            {
                t.HasCheckConstraint("CK_Stores_AverageRating", "\"AverageRating\" >= 0 AND \"AverageRating\" <= 5");
                t.HasCheckConstraint("CK_Stores_RatingsCount", "\"RatingsCount\" >= 0");
                t.HasCheckConstraint("CK_Stores_DeliveryTimeMinutes", "\"DeliveryTimeMinutes\" >= 0");
                t.HasCheckConstraint("CK_Stores_DeliveryFee", "\"DeliveryFee\" >= 0");
            });
            builder.HasKey(s => s.ID);

            builder.Property(s => s.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(s => s.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(s => s.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(s => s.Address).IsRequired().HasMaxLength(300);
            builder.Property(s => s.IconUrl).IsRequired().HasMaxLength(500);
            builder.Property(s => s.DescriptionAr).HasMaxLength(1000);
            builder.Property(s => s.DescriptionEn).HasMaxLength(1000);
            builder.Property(s => s.OpenTime).IsRequired();
            builder.Property(s => s.CloseTime).IsRequired();
            builder.Property(s => s.PriceLevel).IsRequired();
            builder.Property(s => s.AverageRating).IsRequired();
            builder.Property(s => s.RatingsCount).IsRequired();
            builder.Property(s => s.DeliveryTimeMinutes).IsRequired();
            builder.Property(s => s.DeliveryFee).HasPrecision(18, 2).IsRequired();
            builder.Property(s => s.Latitude).IsRequired();
            builder.Property(s => s.Longitude).IsRequired();

            builder.HasOne(s => s.MainCategory)
                   .WithMany()
                   .HasForeignKey(s => s.MainCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(s => s.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.OwnerId, s.IsArchived });

            // Every list query filters by category and/or active state
            builder.HasIndex(s => new { s.MainCategoryId, s.IsArchived });
            builder.HasIndex(s => new { s.IsActive, s.IsArchived });

            // A phone number belongs to one active store
            builder.HasIndex(s => s.PhoneNumber)
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");
        }
    }
}
