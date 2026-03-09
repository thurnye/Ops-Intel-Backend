using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentPackageConfiguration : IEntityTypeConfiguration<ShipmentPackage>
{
    public void Configure(EntityTypeBuilder<ShipmentPackage> builder)
    {
        builder.ToTable("ShipmentPackages", t =>
        {
            t.HasCheckConstraint("CK_ShipmentPackages_Length", "[Length] >= 0");
            t.HasCheckConstraint("CK_ShipmentPackages_Width", "[Width] >= 0");
            t.HasCheckConstraint("CK_ShipmentPackages_Height", "[Height] >= 0");
            t.HasCheckConstraint("CK_ShipmentPackages_Weight", "[Weight] >= 0");
            t.HasCheckConstraint("CK_ShipmentPackages_DeclaredValue", "[DeclaredValue] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PackageNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TrackingNumber).HasMaxLength(100);

        builder.Property(x => x.PackageType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LabelUrl).HasMaxLength(1000);
        builder.Property(x => x.Barcode).HasMaxLength(100);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.Property(x => x.Length).HasPrecision(18, 2);
        builder.Property(x => x.Width).HasPrecision(18, 2);
        builder.Property(x => x.Height).HasPrecision(18, 2);
        builder.Property(x => x.Weight).HasPrecision(18, 4);
        builder.Property(x => x.DeclaredValue).HasPrecision(18, 2);

        builder.HasIndex(x => new { x.ShipmentId, x.PackageNumber }).IsUnique();
        builder.HasIndex(x => x.TrackingNumber);
        builder.HasIndex(x => new { x.ShipmentId, x.Status });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Packages)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PackageItems)
            .WithOne(x => x.ShipmentPackage)
            .HasForeignKey(x => x.ShipmentPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
