using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ResourceCalendarExceptionConfiguration : IEntityTypeConfiguration<ResourceCalendarException>
{
    public void Configure(EntityTypeBuilder<ResourceCalendarException> builder)
    {
        builder.ToTable("ResourceCalendarExceptions", t =>
        {
            t.HasCheckConstraint("CK_ResourceCalendarException_DateRange", "[ExceptionEndUtc] >= [ExceptionStartUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExceptionType)
            .HasConversion<int>();

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.ResourceCalendarId, x.ExceptionStartUtc, x.ExceptionEndUtc });

        builder.HasOne(x => x.ResourceCalendar)
            .WithMany(x => x.Exceptions)
            .HasForeignKey(x => x.ResourceCalendarId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
