using Microsoft.EntityFrameworkCore;
using SewQ.Data.Entities;

namespace SewQ.Data;

public class SewQDbContext(DbContextOptions<SewQDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<LabourEntry> LabourEntries => Set<LabourEntry>();
    public DbSet<Measurement> Measurements => Set<Measurement>();
    public DbSet<MachineSetting> MachineSettings => Set<MachineSetting>();
    public DbSet<AppSettings> Settings => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(e =>
        {
            e.Property(p => p.Name).IsRequired();
            e.Property(p => p.Status).IsRequired();
            e.HasMany(p => p.Materials).WithOne().HasForeignKey(m => m.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(p => p.LabourEntries).WithOne().HasForeignKey(l => l.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(p => p.Measurements).WithOne().HasForeignKey(m => m.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(p => p.MachineSettings).WithOne().HasForeignKey(m => m.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Material>(e => e.Property(m => m.Name).IsRequired());
        modelBuilder.Entity<LabourEntry>(e => e.Property(l => l.Task).IsRequired());
        modelBuilder.Entity<Measurement>(e => e.Property(m => m.Name).IsRequired());
        modelBuilder.Entity<MachineSetting>(e => e.Property(m => m.Title).IsRequired());

        modelBuilder.Entity<AppSettings>().HasData(new AppSettings
        {
            Id = AppSettings.SingletonId,
            Theme = "Atelier",
            DarkMode = false,
            DefaultHourlyRate = 8m,
            Currency = "EUR",
        });
    }
}
