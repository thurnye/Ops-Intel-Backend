using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionLaborLogConfiguration : IEntityTypeConfiguration<ProductionLaborLog>
{
    public void Configure(EntityTypeBuilder<ProductionLaborLog> builder)
    {
        builder.ToTable("ProductionLaborLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HoursWorked)
            .HasPrecision(18, 2);

        builder.Property(x => x.HourlyRate)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => x.ProductionExecutionId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.WorkDate);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany(x => x.LaborLogs)
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
