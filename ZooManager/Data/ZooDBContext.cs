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

        // Many-to-many: Animal <-> Category (via implicit join table)
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Animals)
            .WithMany(); // Animal heeft Category als (optioneel) navigation, maar many-to-many beheren we via join

        // Self-referencing many-to-many: Animal -> Prey
        modelBuilder.Entity<Animal>()
            .HasMany(a => a.Prey)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "AnimalPrey",
                j => j.HasOne<Animal>().WithMany().HasForeignKey("PreyId").OnDelete(DeleteBehavior.Restrict),
                j => j.HasOne<Animal>().WithMany().HasForeignKey("PredatorId").OnDelete(DeleteBehavior.Cascade)
            );

        // Enclosure one-to-many
        modelBuilder.Entity<Enclosure>()
            .HasMany(e => e.Animals)
            .WithOne(a => a.Enclosure)
            .HasForeignKey(a => a.EnclosureId)
            .OnDelete(DeleteBehavior.SetNull);

        // Category optional one-to-many style (Animal.CategoryId is nullable)
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Animals)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}