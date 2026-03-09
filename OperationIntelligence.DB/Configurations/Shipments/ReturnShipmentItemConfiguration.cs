using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ReturnShipmentItemConfiguration : IEntityTypeConfiguration<ReturnShipmentItem>
{
    public void Configure(EntityTypeBuilder<ReturnShipmentItem> builder)
    {
        builder.ToTable("ReturnShipmentItems", t =>
        {
            t.HasCheckConstraint("CK_ReturnShipmentItems_ReturnedQuantity", "[ReturnedQuantity] > 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReturnedQuantity).HasPrecision(18, 4);

        builder.Property(x => x.ReturnCondition).HasMaxLength(100);
        builder.Property(x => x.InspectionResult).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasIndex(x => new { x.ReturnShipmentId, x.ShipmentItemId }).IsUnique();

        builder.HasOne(x => x.ReturnShipment)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ReturnShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ShipmentItem)
            .WithMany(x => x.ReturnItems)
            .HasForeignKey(x => x.ShipmentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
