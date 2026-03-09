using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionDowntimeConfiguration : IEntityTypeConfiguration<ProductionDowntime>
{
    public void Configure(EntityTypeBuilder<ProductionDowntime> builder)
    {
        builder.ToTable("ProductionDowntimes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.ReasonDescription)
            .HasMaxLength(1000);

        builder.Property(x => x.DurationMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsPlanned)
            .HasDefaultValue(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ProductionExecutionId);
        builder.HasIndex(x => x.Reason);
        builder.HasIndex(x => x.StartTime);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany(x => x.Downtimes)
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
