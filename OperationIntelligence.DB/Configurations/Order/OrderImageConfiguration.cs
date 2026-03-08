using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class OrderImageConfiguration : IEntityTypeConfiguration<OrderImage>
{
    public void Configure(EntityTypeBuilder<OrderImage> builder)
    {
        builder.ToTable("OrderImages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.FileExtension).IsRequired().HasMaxLength(20);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(x => x.PublicUrl).HasMaxLength(1000);
        builder.Property(x => x.Caption).HasMaxLength(500);
        builder.Property(x => x.UploadedBy).HasMaxLength(150);
    }
}