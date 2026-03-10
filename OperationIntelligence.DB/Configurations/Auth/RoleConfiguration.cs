using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace OperationIntelligence.DB;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.NormalizedName).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.NormalizedName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(250);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
