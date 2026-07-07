namespace SewQ.Services.Models;

public record ProjectSummaryDto(
    Guid Id,
    string Name,
    ProjectStatus Status,
    decimal MaterialsTotal,
    decimal LabourTotal,
    decimal TotalCost,
    DateTime UpdatedAt);

public record ProjectDetailsDto(
    Guid Id,
    string Name,
    ProjectStatus Status,
    decimal MaterialsTotal,
    decimal LabourTotal,
    decimal TotalCost,
    IReadOnlyList<MeasurementDto> Measurements,
    IReadOnlyList<MaterialDto> Materials,
    IReadOnlyList<LabourDto> Labour,
    IReadOnlyList<MachineSettingDto> MachineSettings);

public record MaterialDto(Guid Id, string Name, decimal Cost, string Notes);

public record LabourDto(Guid Id, string Task, decimal Hours, decimal Rate, decimal Cost, string Notes);

public record MeasurementDto(Guid Id, string Name, string Value, string Unit, string Notes);

public record MachineSettingDto(Guid Id, string Title, string Fabric, string Notes);

public record MaterialInput(string Name, decimal Cost, string Notes);

public record LabourInput(string Task, decimal Hours, decimal Rate, string Notes);

public record MeasurementInput(string Name, string Value, string Unit, string Notes);

public record MachineSettingInput(string Title, string Fabric, string Notes);

public record AppSettingsDto(SewQTheme Theme, bool DarkMode, decimal DefaultHourlyRate, Currency Currency);

public record BackupPayload(string FileName, string Json);
