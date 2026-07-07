namespace SewQ.Data.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = ProjectStatusNames.NotStarted;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Material> Materials { get; } = [];
    public List<LabourEntry> LabourEntries { get; } = [];
    public List<Measurement> Measurements { get; } = [];
    public List<MachineSetting> MachineSettings { get; } = [];
}

/// <summary>
/// Status values stored in the database. Kept as strings so the file stays
/// readable in a SQLite browser and new statuses never renumber old rows.
/// </summary>
public static class ProjectStatusNames
{
    public const string NotStarted = "NotStarted";
    public const string InProgress = "InProgress";
    public const string Finished = "Finished";
    public const string Abandoned = "Abandoned";
}
