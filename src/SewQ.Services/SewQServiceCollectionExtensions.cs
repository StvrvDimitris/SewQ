using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SewQ.Data;
using SewQ.Services.Implementations;
using SewQ.Services.Interfaces;

namespace SewQ.Services;

public static class SewQServiceCollectionExtensions
{
    /// <summary>
    /// Registers everything the UI needs. The app project only ever consumes
    /// the interfaces in SewQ.Services.Interfaces — concrete types stay internal.
    /// </summary>
    public static IServiceCollection AddSewQServices(this IServiceCollection services, string databasePath)
    {
        services.AddDbContextFactory<SewQDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<IProjectService, ProjectService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IBackupService, BackupService>();
        return services;
    }
}
