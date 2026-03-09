using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentAddressConfiguration : IEntityTypeConfiguration<ShipmentAddress>
{
    public void Configure(EntityTypeBuilder<ShipmentAddress> builder)
    {
        builder.ToTable("ShipmentAddresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddressType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ContactName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CompanyName).HasMaxLength(150);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(150);

        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AddressLine2).HasMaxLength(200);

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

        builder.HasIndex(x => new
        {
            x.ContactName,
            x.AddressLine1,
            x.City,
            x.StateOrProvince,
            x.PostalCode,
            x.Country
        });

        builder.HasMany(x => x.OriginShipments)
            .WithOne(x => x.OriginAddress)
            .HasForeignKey(x => x.OriginAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.DestinationShipments)
            .WithOne(x => x.DestinationAddress)
            .HasForeignKey(x => x.DestinationAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.OriginReturnShipments)
            .WithOne(x => x.OriginAddress)
            .HasForeignKey(x => x.OriginAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.DestinationReturnShipments)
            .WithOne(x => x.DestinationAddress)
            .HasForeignKey(x => x.DestinationAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
