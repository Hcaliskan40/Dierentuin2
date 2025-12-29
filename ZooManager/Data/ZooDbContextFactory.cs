using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZooManager.Data;

public class ZooDbContextFactory : IDesignTimeDbContextFactory<ZooDbContext>
{
    public ZooDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ZooDbContext>();

        var connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=Dierentuin5;Trusted_Connection=True;MultipleActiveResultSets=true";

        optionsBuilder.UseSqlServer(connectionString);

        return new ZooDbContext(optionsBuilder.Options);
    }
}