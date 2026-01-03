using Microsoft.EntityFrameworkCore;
using ZooManager.Models;

namespace ZooManager.Data;

public class ZooDbContext : DbContext
{
    public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options) { }

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enclosure> Enclosures => Set<Enclosure>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Animal>(b =>
        {
            b.Property(a => a.Name).HasMaxLength(100).IsRequired();
            b.Property(a => a.Species).HasMaxLength(100).IsRequired();

            // Category: NULL allowed
            b.HasOne(a => a.Category)
                .WithMany(c => c.Animals)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Enclosure: NULL allowed
            b.HasOne(a => a.Enclosure)
                .WithMany(e => e.Animals)
                .HasForeignKey(a => a.EnclosureId)
                .OnDelete(DeleteBehavior.SetNull);

            // Prey self-reference: NO ACTION to avoid multiple cascade paths in SQL Server
            b.HasOne(a => a.Prey)
                .WithMany()
                .HasForeignKey(a => a.PreyId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.Property(c => c.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Enclosure>(b =>
        {
            b.Property(e => e.Name).HasMaxLength(120).IsRequired();
        });
    }
}