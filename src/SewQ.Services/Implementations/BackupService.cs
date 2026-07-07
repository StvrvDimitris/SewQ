using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SewQ.Data;
using SewQ.Services.Interfaces;
using SewQ.Services.Models;

namespace SewQ.Services.Implementations;

internal sealed class BackupService(IDbContextFactory<SewQDbContext> dbFactory, ISettingsService settings) : IBackupService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<BackupPayload> CreateBackupAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var projects = await db.Projects.AsNoTracking()
            .Include(p => p.Materials)
            .Include(p => p.LabourEntries)
            .Include(p => p.Measurements)
            .Include(p => p.MachineSettings)
            .AsSplitQuery()
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        var payload = new
        {
            App = "SewQ",
            Version = 1,
            ExportedAt = DateTime.UtcNow,
            Settings = await settings.GetAsync(),
            Projects = projects.Select(p => new
            {
                p.Id,
                p.Name,
                Status = EnumMap.ToStatus(p.Status),
                p.CreatedAt,
                p.UpdatedAt,
                Measurements = p.Measurements.OrderBy(m => m.CreatedAt)
                    .Select(m => new { m.Id, m.Name, m.Value, m.Unit, m.Notes }),
                Materials = p.Materials.OrderBy(m => m.CreatedAt)
                    .Select(m => new { m.Id, m.Name, m.Cost, m.Notes }),
                Labour = p.LabourEntries.OrderBy(l => l.CreatedAt)
                    .Select(l => new { l.Id, l.Task, l.Hours, l.Rate, Cost = l.Hours * l.Rate, l.Notes }),
                MachineSettings = p.MachineSettings.OrderBy(m => m.CreatedAt)
                    .Select(m => new { m.Id, m.Title, m.Fabric, m.Notes }),
            }),
        };

        var fileName = $"sewq-backup-{DateTime.Now:yyyy-MM-dd-HHmm}.json";
        return new BackupPayload(fileName, JsonSerializer.Serialize(payload, JsonOptions));
    }
}
