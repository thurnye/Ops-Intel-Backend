using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class VendorBillLineConfiguration : IEntityTypeConfiguration<VendorBillLine>
{
    public void Configure(EntityTypeBuilder<VendorBillLine> builder)
    {
        builder.ToTable("VendorBillLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LineNumber).IsRequired();
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UnitCost).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.LineTotal).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => new { x.VendorBillId, x.LineNumber }).IsUnique();
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.ShipmentItemId);
        builder.HasIndex(x => x.ProductionMaterialId);

        builder.HasOne(x => x.VendorBill)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.VendorBillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
