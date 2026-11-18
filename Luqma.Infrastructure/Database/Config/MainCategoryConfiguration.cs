using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class MainCategoryConfiguration : IEntityTypeConfiguration<MainCategory>
    {
        public void Configure(EntityTypeBuilder<MainCategory> builder)
        {
            builder.ToTable("MainCategories");
            builder.HasKey(c => c.ID);

            builder.Property(c => c.NameAr).IsRequired().HasMaxLength(MaxNameLength);
            builder.Property(c => c.NameEn).IsRequired().HasMaxLength(MaxNameLength);
            builder.Property(c => c.IconUrl).IsRequired().HasMaxLength(MaxIconUrlLength);

            // One row per name; archived rows are excluded so an archived name can be reused
            builder.HasIndex(c => c.NameAr)
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");

            // Every list query is "active categories ordered for display"
            builder.HasIndex(c => new { c.IsArchived, c.DisplayOrder });

            builder.HasData(
                new MainCategory
                {
                    ID = Guid.Parse("6f1c7b8e-1a2b-4c3d-8e4f-000000000001"),
                    NameAr = "مطاعم",
                    NameEn = "Restaurants",
                    IconUrl = "/icons/categories/restaurants.png",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MainCategory
                {
                    ID = Guid.Parse("6f1c7b8e-1a2b-4c3d-8e4f-000000000002"),
                    NameAr = "صيدلية",
                    NameEn = "Pharmacy",
                    IconUrl = "/icons/categories/pharmacy.png",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MainCategory
                {
                    ID = Guid.Parse("6f1c7b8e-1a2b-4c3d-8e4f-000000000003"),
                    NameAr = "حلويات",
                    NameEn = "Sweets",
                    IconUrl = "/icons/categories/sweets.png",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new MainCategory
                {
                    ID = Guid.Parse("6f1c7b8e-1a2b-4c3d-8e4f-000000000004"),
                    NameAr = "مشروبات",
                    NameEn = "Beverages",
                    IconUrl = "/icons/categories/beverages.png",
                    DisplayOrder = 4,
                    IsActive = true
                });
        }

        private const int MaxNameLength = 100;
        private const int MaxIconUrlLength = 500;
    }
}
