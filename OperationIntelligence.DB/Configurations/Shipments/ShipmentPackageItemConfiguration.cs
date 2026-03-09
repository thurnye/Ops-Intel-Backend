using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentPackageItemConfiguration : IEntityTypeConfiguration<ShipmentPackageItem>
{
    public void Configure(EntityTypeBuilder<ShipmentPackageItem> builder)
    {
        builder.ToTable("ShipmentPackageItems", t =>
        {
            t.HasCheckConstraint("CK_ShipmentPackageItems_Quantity", "[Quantity] > 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity).HasPrecision(18, 4);

        builder.HasIndex(x => new { x.ShipmentPackageId, x.ShipmentItemId }).IsUnique();

        builder.HasOne(x => x.ShipmentPackage)
            .WithMany(x => x.PackageItems)
            .HasForeignKey(x => x.ShipmentPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ShipmentItem)
            .WithMany(x => x.PackageItems)
            .HasForeignKey(x => x.ShipmentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
