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

        // Table names (optioneel, maar netjes)
        modelBuilder.Entity<Animal>().ToTable("Animals");
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<Enclosure>().ToTable("Enclosures");

        // Animal -> Category (optioneel)
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Animals)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Animal -> Enclosure (optioneel)
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Enclosure)
            .WithMany(e => e.Animals)
            .HasForeignKey(a => a.EnclosureId)
            .OnDelete(DeleteBehavior.SetNull);

        // Animal -> Prey (self reference, optioneel)
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Prey)
            .WithMany()
            .HasForeignKey(a => a.PreyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}