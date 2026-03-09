using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleOperationResourceOptionConfiguration : IEntityTypeConfiguration<ScheduleOperationResourceOption>
{
    public void Configure(EntityTypeBuilder<ScheduleOperationResourceOption> builder)
    {
        builder.ToTable("ScheduleOperationResourceOptions", t =>
        {
            t.HasCheckConstraint("CK_ScheduleOperationResourceOption_PreferenceRank", "[PreferenceRank] > 0");
            t.HasCheckConstraint("CK_ScheduleOperationResourceOption_EfficiencyFactor", "[EfficiencyFactor] > 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceType)
            .HasConversion<int>();

        builder.Property(x => x.EfficiencyFactor)
            .HasPrecision(8, 4);

        builder.Property(x => x.SetupPenaltyMinutes)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new { x.ScheduleOperationId, x.PreferenceRank });

        builder.HasIndex(x => new { x.ScheduleOperationId, x.ResourceId, x.ResourceType });

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.ResourceOptions)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
