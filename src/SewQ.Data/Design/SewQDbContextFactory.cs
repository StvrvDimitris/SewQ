using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SewQ.Data.Design;

/// <summary>
/// Used only by the dotnet-ef CLI to build the model when scaffolding
/// migrations; the connection string is never opened at design time.
/// </summary>
public sealed class SewQDbContextFactory : IDesignTimeDbContextFactory<SewQDbContext>
{
    public SewQDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SewQDbContext>()
            .UseSqlite("Data Source=sewq-design.db3")
            .Options;
        return new SewQDbContext(options);
    }
}
