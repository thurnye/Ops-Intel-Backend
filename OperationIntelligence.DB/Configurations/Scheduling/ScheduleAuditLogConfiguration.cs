using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleAuditLogConfiguration : IEntityTypeConfiguration<ScheduleAuditLog>
{
    public void Configure(EntityTypeBuilder<ScheduleAuditLog> builder)
    {
        builder.ToTable("ScheduleAuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ActionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ChangedFieldsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.OldValuesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValuesJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.EntityName, x.EntityId });
        builder.HasIndex(x => x.PerformedAtUtc);
        builder.HasIndex(x => x.CorrelationId);
    }
}
