using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FromStatus)
            .IsRequired();

        builder.Property(x => x.ToStatus)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.ChangedBy)
            .HasMaxLength(150);

        builder.Property(x => x.ChangedAtUtc)
            .IsRequired();

        builder.Property(x => x.Comments)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(150);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(150);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.ChangedAtUtc);
        builder.HasIndex(x => new { x.OrderId, x.ToStatus, x.ChangedAtUtc });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}