using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentInsuranceConfiguration : IEntityTypeConfiguration<ShipmentInsurance>
{
    public void Configure(EntityTypeBuilder<ShipmentInsurance> builder)
    {
        builder.ToTable("ShipmentInsurances", t =>
        {
            t.HasCheckConstraint("CK_ShipmentInsurances_InsuredAmount", "[InsuredAmount] >= 0");
            t.HasCheckConstraint("CK_ShipmentInsurances_PremiumAmount", "[PremiumAmount] >= 0");
            t.HasCheckConstraint(
                "CK_ShipmentInsurances_DateRange",
                "[ExpiryDateUtc] IS NULL OR [ExpiryDateUtc] >= [EffectiveDateUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProviderName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PolicyNumber).HasMaxLength(100);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.Property(x => x.InsuredAmount).HasPrecision(18, 2);
        builder.Property(x => x.PremiumAmount).HasPrecision(18, 2);

        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => x.PolicyNumber);

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Insurances)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
