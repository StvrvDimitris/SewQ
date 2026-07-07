using Microsoft.EntityFrameworkCore;
using SewQ.Data;
using SewQ.Data.Entities;
using SewQ.Services.Interfaces;
using SewQ.Services.Models;

namespace SewQ.Services.Implementations;

internal sealed class ProjectService(IDbContextFactory<SewQDbContext> dbFactory) : IProjectService
{
    public async Task<IReadOnlyList<ProjectSummaryDto>> GetProjectSummariesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        // Totals are computed in memory: SQLite cannot aggregate decimals in SQL,
        // and a home sewing catalogue is small enough that this never matters.
        var projects = await db.Projects.AsNoTracking()
            .Include(p => p.Materials)
            .Include(p => p.LabourEntries)
            .AsSplitQuery()
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();

        return projects.Select(p =>
        {
            var materials = p.Materials.Sum(m => m.Cost);
            var labour = p.LabourEntries.Sum(l => l.Hours * l.Rate);
            return new ProjectSummaryDto(
                p.Id, p.Name, EnumMap.ToStatus(p.Status),
                materials, labour, materials + labour, p.UpdatedAt);
        }).ToList();
    }

    public async Task<ProjectDetailsDto?> GetProjectAsync(Guid projectId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var project = await db.Projects.AsNoTracking()
            .Include(p => p.Materials)
            .Include(p => p.LabourEntries)
            .Include(p => p.Measurements)
            .Include(p => p.MachineSettings)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == projectId);
        if (project is null)
            return null;

        var materialsTotal = project.Materials.Sum(m => m.Cost);
        var labourTotal = project.LabourEntries.Sum(l => l.Hours * l.Rate);

        return new ProjectDetailsDto(
            project.Id,
            project.Name,
            EnumMap.ToStatus(project.Status),
            materialsTotal,
            labourTotal,
            materialsTotal + labourTotal,
            project.Measurements.OrderBy(m => m.CreatedAt)
                .Select(m => new MeasurementDto(m.Id, m.Name, m.Value, m.Unit, m.Notes)).ToList(),
            project.Materials.OrderBy(m => m.CreatedAt)
                .Select(m => new MaterialDto(m.Id, m.Name, m.Cost, m.Notes)).ToList(),
            project.LabourEntries.OrderBy(l => l.CreatedAt)
                .Select(l => new LabourDto(l.Id, l.Task, l.Hours, l.Rate, l.Hours * l.Rate, l.Notes)).ToList(),
            project.MachineSettings.OrderBy(m => m.CreatedAt)
                .Select(m => new MachineSettingDto(m.Id, m.Title, m.Fabric, m.Notes)).ToList());
    }

    public async Task<Guid> CreateProjectAsync(string name, ProjectStatus status)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Status = status.ToString(),
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return project.Id;
    }

    public async Task SetStatusAsync(Guid projectId, ProjectStatus status)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var project = await GetRequiredProjectAsync(db, projectId);
        project.Status = status.ToString();
        project.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task AddMaterialAsync(Guid projectId, MaterialInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Materials.Add(new Material
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = input.Name.Trim(),
            Cost = input.Cost,
            Notes = input.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
        });
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task UpdateMaterialAsync(Guid projectId, Guid materialId, MaterialInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var material = await db.Materials.FirstAsync(m => m.Id == materialId && m.ProjectId == projectId);
        material.Name = input.Name.Trim();
        material.Cost = input.Cost;
        material.Notes = input.Notes.Trim();
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task DeleteMaterialAsync(Guid projectId, Guid materialId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var material = await db.Materials.FirstAsync(m => m.Id == materialId && m.ProjectId == projectId);
        db.Materials.Remove(material);
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task AddLabourAsync(Guid projectId, LabourInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.LabourEntries.Add(new LabourEntry
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Task = input.Task.Trim(),
            Hours = input.Hours,
            Rate = input.Rate,
            Notes = input.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
        });
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task UpdateLabourAsync(Guid projectId, Guid labourId, LabourInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var labour = await db.LabourEntries.FirstAsync(l => l.Id == labourId && l.ProjectId == projectId);
        labour.Task = input.Task.Trim();
        labour.Hours = input.Hours;
        labour.Rate = input.Rate;
        labour.Notes = input.Notes.Trim();
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task DeleteLabourAsync(Guid projectId, Guid labourId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var labour = await db.LabourEntries.FirstAsync(l => l.Id == labourId && l.ProjectId == projectId);
        db.LabourEntries.Remove(labour);
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task AddMeasurementAsync(Guid projectId, MeasurementInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Measurements.Add(new Measurement
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = input.Name.Trim(),
            Value = input.Value.Trim(),
            Unit = input.Unit.Trim(),
            Notes = input.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
        });
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task UpdateMeasurementAsync(Guid projectId, Guid measurementId, MeasurementInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var measurement = await db.Measurements.FirstAsync(m => m.Id == measurementId && m.ProjectId == projectId);
        measurement.Name = input.Name.Trim();
        measurement.Value = input.Value.Trim();
        measurement.Unit = input.Unit.Trim();
        measurement.Notes = input.Notes.Trim();
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task DeleteMeasurementAsync(Guid projectId, Guid measurementId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var measurement = await db.Measurements.FirstAsync(m => m.Id == measurementId && m.ProjectId == projectId);
        db.Measurements.Remove(measurement);
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task AddMachineSettingAsync(Guid projectId, MachineSettingInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.MachineSettings.Add(new MachineSetting
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = input.Title.Trim(),
            Fabric = input.Fabric.Trim(),
            Notes = input.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
        });
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task UpdateMachineSettingAsync(Guid projectId, Guid machineSettingId, MachineSettingInput input)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var setting = await db.MachineSettings.FirstAsync(m => m.Id == machineSettingId && m.ProjectId == projectId);
        setting.Title = input.Title.Trim();
        setting.Fabric = input.Fabric.Trim();
        setting.Notes = input.Notes.Trim();
        await TouchAndSaveAsync(db, projectId);
    }

    public async Task DeleteMachineSettingAsync(Guid projectId, Guid machineSettingId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var setting = await db.MachineSettings.FirstAsync(m => m.Id == machineSettingId && m.ProjectId == projectId);
        db.MachineSettings.Remove(setting);
        await TouchAndSaveAsync(db, projectId);
    }

    private static async Task<Project> GetRequiredProjectAsync(SewQDbContext db, Guid projectId)
        => await db.Projects.FirstAsync(p => p.Id == projectId);

    private static async Task TouchAndSaveAsync(SewQDbContext db, Guid projectId)
    {
        var project = await GetRequiredProjectAsync(db, projectId);
        project.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }
}
