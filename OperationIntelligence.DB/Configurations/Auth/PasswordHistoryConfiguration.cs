using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace OperationIntelligence.DB;

public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("PasswordHistories");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);

        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordHistories)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
