namespace SewQ.Data.Entities;

public class LabourEntry
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Task { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal Rate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
