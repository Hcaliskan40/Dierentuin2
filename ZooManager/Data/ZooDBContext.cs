using Microsoft.EntityFrameworkCore;
using ZooManager.Models;

namespace ZooManager.Data;

public class ZooDbContext : DbContext
{
    public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options)
    {
    }

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enclosure> Enclosures => Set<Enclosure>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ FIX: self-referencing FK mag NIET cascade (SET NULL is ook cascade in SQL Server)
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Prey)
            .WithMany()
            .HasForeignKey(a => a.PreyId)
            .OnDelete(DeleteBehavior.NoAction);

        // Deze mogen wel SET NULL, maar NO ACTION is ook prima/veilig:
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Animals)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Enclosure)
            .WithMany(e => e.Animals)
            .HasForeignKey(a => a.EnclosureId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}