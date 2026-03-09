using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class CarrierServiceConfiguration : IEntityTypeConfiguration<CarrierService>
{
    public void Configure(EntityTypeBuilder<CarrierService> builder)
    {
        builder.ToTable("CarrierServices", t =>
        {
            t.HasCheckConstraint("CK_CarrierServices_EstimatedTransitDays", "[EstimatedTransitDays] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasIndex(x => new { x.CarrierId, x.ServiceCode }).IsUnique();
        builder.HasIndex(x => new { x.CarrierId, x.Name });

        builder.HasOne(x => x.Carrier)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Shipments)
            .WithOne(x => x.CarrierService)
            .HasForeignKey(x => x.CarrierServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ReturnShipments)
            .WithOne(x => x.CarrierService)
            .HasForeignKey(x => x.CarrierServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
