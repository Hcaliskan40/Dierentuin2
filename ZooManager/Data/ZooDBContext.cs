using Microsoft.EntityFrameworkCore;
using ZooManager.Models;

namespace ZooManager.Data;

public class ZooDbContext : DbContext
{
    public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options) { }

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enclosure> Enclosures => Set<Enclosure>();
}
