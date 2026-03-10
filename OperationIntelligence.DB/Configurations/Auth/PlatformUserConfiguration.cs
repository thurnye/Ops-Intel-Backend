using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace OperationIntelligence.DB;

public class PlatformUserConfiguration : IEntityTypeConfiguration<PlatformUser>
{
    public void Configure(EntityTypeBuilder<PlatformUser> builder)
    {
        builder.ToTable("PlatformUsers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.NormalizedEmail).IsRequired().HasMaxLength(256);
        builder.Property(x => x.UserName).HasMaxLength(100);
        builder.Property(x => x.NormalizedUserName).HasMaxLength(100);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(x => x.PasswordSalt).HasMaxLength(128);
        builder.Property(x => x.AuthProvider).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ExternalProviderId).HasMaxLength(200);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
        builder.HasIndex(x => x.UserName).IsUnique(false);
        builder.HasIndex(x => x.NormalizedUserName).IsUnique(false);
        builder.HasIndex(x => x.IsDeleted);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.EmailConfirmed);

        builder.HasOne(x => x.Profile)
            .WithOne(x => x.User)
            .HasForeignKey<PlatformUserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
