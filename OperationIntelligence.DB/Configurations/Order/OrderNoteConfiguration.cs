using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class OrderNoteConfiguration : IEntityTypeConfiguration<OrderNote>
{
    public void Configure(EntityTypeBuilder<OrderNote> builder)
    {
        builder.ToTable("OrderNotes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Note)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.IsInternal)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(150);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(150);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderNotes)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
