using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace OperationIntelligence.DB;

public class PlatformUserProfileConfiguration : IEntityTypeConfiguration<PlatformUserProfile>
{
    public void Configure(EntityTypeBuilder<PlatformUserProfile> builder)
    {
        builder.ToTable("PlatformUserProfiles");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.AvatarFileId);

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DisplayName).HasMaxLength(200);
        builder.Property(x => x.Gender).HasMaxLength(50);
        builder.Property(x => x.PhoneNumber).HasMaxLength(50);
        builder.Property(x => x.AddressLine1).HasMaxLength(200);
        builder.Property(x => x.AddressLine2).HasMaxLength(200);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.StateOrProvince).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.PostalCode).HasMaxLength(20);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasOne(x => x.User)
            .WithOne(x => x.Profile)
            .HasForeignKey<PlatformUserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AvatarFile)
            .WithMany()
            .HasForeignKey(x => x.AvatarFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
