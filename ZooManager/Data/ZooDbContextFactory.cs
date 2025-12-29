using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZooManager.Data;

public class ZooDbContextFactory : IDesignTimeDbContextFactory<ZooDbContext>
{
    public ZooDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ZooDbContext>();
        optionsBuilder.UseSqlite("Data Source=zoo.db");
        return new ZooDbContext(optionsBuilder.Options);
    }
}