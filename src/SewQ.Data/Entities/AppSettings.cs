namespace SewQ.Data.Entities;

/// <summary>
/// Single-row table; always addressed via <see cref="SingletonId"/>.
/// </summary>
public class AppSettings
{
    public static readonly Guid SingletonId = new("a1e5b9c2-0d34-4f6a-8b7e-3c2d1a0f9e88");

    public Guid Id { get; set; }
    public string Theme { get; set; } = "Atelier";
    public bool DarkMode { get; set; }
    public decimal DefaultHourlyRate { get; set; }
    public string Currency { get; set; } = "EUR";
}
