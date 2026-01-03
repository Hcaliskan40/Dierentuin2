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

        // Animal
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.Property(a => a.Name).HasMaxLength(100).IsRequired();
            entity.Property(a => a.Species).HasMaxLength(100).IsRequired();

            // Category relatie: SET NULL is prima
            entity.HasOne(a => a.Category)
                .WithMany(c => c.Animals)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Enclosure relatie: SET NULL is prima
            entity.HasOne(a => a.Enclosure)
                .WithMany(e => e.Animals)
                .HasForeignKey(a => a.EnclosureId)
                .OnDelete(DeleteBehavior.SetNull);

            // Prey relatie (self reference): GEEN cascade/SET NULL, anders "multiple cascade paths"
            entity.HasOne(a => a.Prey)
                .WithMany()
                .HasForeignKey(a => a.PreyId)
                .OnDelete(DeleteBehavior.NoAction); // <-- dit fixt Brandon's error
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
        });

        // Enclosure
        modelBuilder.Entity<Enclosure>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });
    }
}