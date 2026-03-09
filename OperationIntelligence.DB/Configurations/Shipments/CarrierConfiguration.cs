using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
{
    public void Configure(EntityTypeBuilder<Carrier> builder)
    {
        builder.ToTable("Carriers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CarrierCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.ContactName).HasMaxLength(150);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.Website).HasMaxLength(500);

        builder.HasIndex(x => x.CarrierCode).IsUnique();
        builder.HasIndex(x => x.Name);

        builder.HasMany(x => x.Services)
            .WithOne(x => x.Carrier)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Shipments)
            .WithOne(x => x.Carrier)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.DockAppointments)
            .WithOne(x => x.Carrier)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ReturnShipments)
            .WithOne(x => x.Carrier)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
