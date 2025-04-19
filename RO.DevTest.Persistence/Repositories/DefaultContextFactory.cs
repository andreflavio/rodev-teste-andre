using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RO.DevTest.Persistence;

public class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=DevTestDb;Username=postgres;Password=AFO123381");

        return new DefaultContext(optionsBuilder.Options);
    }
}
