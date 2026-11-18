using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class StoreImageConfiguration : IEntityTypeConfiguration<StoreImage>
    {
        public void Configure(EntityTypeBuilder<StoreImage> builder)
        {
            builder.ToTable("StoreImages");
            builder.HasKey(i => i.ID);

            builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);

            builder.HasOne(i => i.Store)
                   .WithMany(s => s.Images)
                   .HasForeignKey(i => i.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);

            // The gallery is always read per store, in display order
            builder.HasIndex(i => new { i.StoreId, i.IsArchived });
        }
    }
}
