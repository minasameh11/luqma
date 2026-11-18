using Luqma.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luqma.Infrastructure.Database.Config
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.HasKey(a => a.ID);

            builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
            builder.Property(a => a.District).HasMaxLength(100);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Details).HasMaxLength(500);
            builder.Property(a => a.Label).HasConversion<int>();

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Every address query is "my active addresses"
            builder.HasIndex(a => new { a.UserId, a.IsArchived });
        }
    }
}
