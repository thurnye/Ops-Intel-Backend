using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts", t =>
        {
            t.HasCheckConstraint("CK_Shift_CapacityMinutes", "[CapacityMinutes] >= 0");
            t.HasCheckConstraint("CK_Shift_BreakMinutes", "[BreakMinutes] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShiftCode)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.ShiftName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.WarehouseId, x.ShiftCode });

        builder.HasIndex(x => new { x.WorkCenterId, x.IsActive });

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkCenter)
            .WithMany()
            .HasForeignKey(x => x.WorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
