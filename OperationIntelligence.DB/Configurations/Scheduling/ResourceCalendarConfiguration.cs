using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ResourceCalendarConfiguration : IEntityTypeConfiguration<ResourceCalendar>
{
    public void Configure(EntityTypeBuilder<ResourceCalendar> builder)
    {
        builder.ToTable("ResourceCalendars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceType)
            .HasConversion<int>();

        builder.Property(x => x.CalendarName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.TimeZone)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.ResourceId, x.ResourceType, x.IsDefault });

        builder.HasMany(x => x.Exceptions)
            .WithOne(x => x.ResourceCalendar)
            .HasForeignKey(x => x.ResourceCalendarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
