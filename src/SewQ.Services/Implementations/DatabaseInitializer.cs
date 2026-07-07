using Microsoft.EntityFrameworkCore;
using SewQ.Data;
using SewQ.Services.Interfaces;

namespace SewQ.Services.Implementations;

internal sealed class DatabaseInitializer(IDbContextFactory<SewQDbContext> dbFactory) : IDatabaseInitializer
{
    public void Initialize()
    {
        using var db = dbFactory.CreateDbContext();
        db.Database.Migrate();
    }
}
