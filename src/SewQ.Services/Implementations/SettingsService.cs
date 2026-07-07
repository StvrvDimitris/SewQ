using Microsoft.EntityFrameworkCore;
using SewQ.Data;
using SewQ.Data.Entities;
using SewQ.Services.Interfaces;
using SewQ.Services.Models;

namespace SewQ.Services.Implementations;

internal sealed class SettingsService(IDbContextFactory<SewQDbContext> dbFactory) : ISettingsService
{
    public async Task<AppSettingsDto> GetAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var row = await db.Settings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == AppSettings.SingletonId);
        return row is null
            ? new AppSettingsDto(SewQTheme.Atelier, false, 8m, Currency.EUR)
            : new AppSettingsDto(
                EnumMap.ToTheme(row.Theme),
                row.DarkMode,
                row.DefaultHourlyRate,
                EnumMap.ToCurrency(row.Currency));
    }

    public async Task SaveAsync(AppSettingsDto settings)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var row = await db.Settings.FirstOrDefaultAsync(s => s.Id == AppSettings.SingletonId);
        if (row is null)
        {
            row = new AppSettings { Id = AppSettings.SingletonId };
            db.Settings.Add(row);
        }
        row.Theme = settings.Theme.ToString();
        row.DarkMode = settings.DarkMode;
        row.DefaultHourlyRate = settings.DefaultHourlyRate;
        row.Currency = settings.Currency.ToString();
        await db.SaveChangesAsync();
    }
}
