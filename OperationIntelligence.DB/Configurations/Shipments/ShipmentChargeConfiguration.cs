using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentChargeConfiguration : IEntityTypeConfiguration<ShipmentCharge>
{
    public void Configure(EntityTypeBuilder<ShipmentCharge> builder)
    {
        builder.ToTable("ShipmentCharges", t =>
        {
            t.HasCheckConstraint("CK_ShipmentCharges_Amount", "[Amount] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChargeType).HasConversion<int>();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => new { x.ShipmentId, x.ChargeType });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Charges)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
