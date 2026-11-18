using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class StoreCuisineConfiguration : IEntityTypeConfiguration<StoreCuisine>
    {
        public void Configure(EntityTypeBuilder<StoreCuisine> builder)
        {
            builder.ToTable("StoreCuisines");
            builder.HasKey(sc => sc.ID);

            builder.HasOne(sc => sc.Store)
                   .WithMany()
                   .HasForeignKey(sc => sc.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sc => sc.Cuisine)
                   .WithMany()
                   .HasForeignKey(sc => sc.CuisineId)
                   .OnDelete(DeleteBehavior.Restrict);

            // A store can be linked to a cuisine once, and the customer filter is by cuisine
            builder.HasIndex(sc => new { sc.StoreId, sc.CuisineId })
                   .IsUnique()
                   .HasFilter("\"IsArchived\" = false");

            builder.HasIndex(sc => new { sc.CuisineId, sc.IsArchived });
        }
    }
}
