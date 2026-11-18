using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class CuisineConfiguration : IEntityTypeConfiguration<Cuisine>
    {
        public void Configure(EntityTypeBuilder<Cuisine> builder)
        {
            builder.ToTable("Cuisines");
            builder.HasKey(c => c.ID);

            builder.Property(c => c.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(c => c.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ImageUrl).HasMaxLength(500);

            // One row per name; archived rows are excluded so an archived name can be reused
            builder.HasIndex(c => c.NameAr)
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");

            // Filter chips are listed in display order
            builder.HasIndex(c => new { c.IsArchived, c.IsActive, c.DisplayOrder });

            builder.HasData(
                new Cuisine
                {
                    ID = Guid.Parse("7a2d4c31-5b6e-4f80-9c11-000000000001"),
                    NameAr = "مشويات",
                    NameEn = "Grill",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Cuisine
                {
                    ID = Guid.Parse("7a2d4c31-5b6e-4f80-9c11-000000000002"),
                    NameAr = "أسماك",
                    NameEn = "Seafood",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Cuisine
                {
                    ID = Guid.Parse("7a2d4c31-5b6e-4f80-9c11-000000000003"),
                    NameAr = "شرقي",
                    NameEn = "Eastern",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new Cuisine
                {
                    ID = Guid.Parse("7a2d4c31-5b6e-4f80-9c11-000000000004"),
                    NameAr = "غربي",
                    NameEn = "Western",
                    DisplayOrder = 4,
                    IsActive = true
                });
        }
    }
}
