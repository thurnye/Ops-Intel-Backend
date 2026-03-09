using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class CustomsDocumentConfiguration : IEntityTypeConfiguration<CustomsDocument>
{
    public void Configure(EntityTypeBuilder<CustomsDocument> builder)
    {
        builder.ToTable("CustomsDocuments", t =>
        {
            t.HasCheckConstraint("CK_CustomsDocuments_DeclaredCustomsValue", "[DeclaredCustomsValue] IS NULL OR [DeclaredCustomsValue] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentType).HasConversion<int>();

        builder.Property(x => x.DocumentNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.CountryOfOrigin).HasMaxLength(100);
        builder.Property(x => x.DestinationCountry).HasMaxLength(100);
        builder.Property(x => x.HarmonizedCode).HasMaxLength(50);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.DeclaredCustomsValue).HasPrecision(18, 2);

        builder.HasIndex(x => x.DocumentNumber);
        builder.HasIndex(x => new { x.ShipmentId, x.DocumentType });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.CustomsDocuments)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
