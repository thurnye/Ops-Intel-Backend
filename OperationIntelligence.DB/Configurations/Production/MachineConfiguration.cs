using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.ToTable("Machines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MachineCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Model)
            .HasMaxLength(100);

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(100);

        builder.Property(x => x.SerialNumber)
            .HasMaxLength(100);

        builder.Property(x => x.HourlyRunningCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(x => x.MachineCode)
            .IsUnique();

        builder.HasIndex(x => x.WorkCenterId);

        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.WorkCenter)
            .WithMany(x => x.Machines)
            .HasForeignKey(x => x.WorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
