using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class OrderAddressConfiguration : IEntityTypeConfiguration<OrderAddress>
{
    public void Configure(EntityTypeBuilder<OrderAddress> builder)
    {
        builder.ToTable("OrderAddresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddressType)
            .IsRequired();

        builder.Property(x => x.ContactName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(150);

        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(200);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.StateOrProvince)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(150);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(150);

        builder.HasIndex(x => new { x.OrderId, x.AddressType });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}