using OperationIntelligence.DB.Entities;
using Microsoft.EntityFrameworkCore;


namespace OperationIntelligence.DB
{
    public class OperationIntelligenceDbContext : DbContext
    {
        public OperationIntelligenceDbContext(DbContextOptions<OperationIntelligenceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Donation> Donations { get; set; }
        public DbSet<PlatformUser> Users => Set<PlatformUser>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Donation>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Description).IsRequired();
                entity.Property(d => d.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PlatformUser>(entity =>
            {
                entity.ToTable("ApplicationUser");
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(256);
                entity.HasIndex(rt => rt.Token).IsUnique();

                // Configure the relationship to PlatformUser
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
