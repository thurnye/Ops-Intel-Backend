using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace OperationIntelligence.DB;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.FileName);
        builder.HasIndex(x => x.ChecksumSha256);

        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.StoragePath).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.PublicUrl).HasMaxLength(1000);
        builder.Property(x => x.ChecksumSha256).HasMaxLength(128);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
