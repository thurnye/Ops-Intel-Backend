using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentExceptionConfiguration : IEntityTypeConfiguration<ShipmentException>
{
    public void Configure(EntityTypeBuilder<ShipmentException> builder)
    {
        builder.ToTable("ShipmentExceptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExceptionType).HasConversion<int>();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.ReportedBy).HasMaxLength(150);
        builder.Property(x => x.ResolutionNote).HasMaxLength(1000);

        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => new { x.ShipmentId, x.ExceptionType });
        builder.HasIndex(x => new { x.IsResolved, x.ReportedAtUtc });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Exceptions)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
