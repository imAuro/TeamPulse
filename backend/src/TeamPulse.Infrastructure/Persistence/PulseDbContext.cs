using Microsoft.EntityFrameworkCore;
using TeamPulse.Domain.Pulses;

namespace TeamPulse.Infrastructure.Persistence;

public sealed class PulseDbContext(DbContextOptions<PulseDbContext> options) : DbContext(options)
{
    public DbSet<PulseEntry> PulseEntries => Set<PulseEntry>();
    public DbSet<PulseCategory> PulseCategories => Set<PulseCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PulseCategory>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<PulseEntry>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Score).IsRequired();
            b.Property(x => x.Comment).HasMaxLength(PulseEntry.MaxCommentLength);
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasOne<PulseCategory>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}