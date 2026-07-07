using SewQ.Services.Models;

namespace SewQ.Services.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectSummaryDto>> GetProjectSummariesAsync();
    Task<ProjectDetailsDto?> GetProjectAsync(Guid projectId);
    Task<Guid> CreateProjectAsync(string name, ProjectStatus status);
    Task SetStatusAsync(Guid projectId, ProjectStatus status);

    Task AddMaterialAsync(Guid projectId, MaterialInput input);
    Task UpdateMaterialAsync(Guid projectId, Guid materialId, MaterialInput input);
    Task DeleteMaterialAsync(Guid projectId, Guid materialId);

    Task AddLabourAsync(Guid projectId, LabourInput input);
    Task UpdateLabourAsync(Guid projectId, Guid labourId, LabourInput input);
    Task DeleteLabourAsync(Guid projectId, Guid labourId);

    Task AddMeasurementAsync(Guid projectId, MeasurementInput input);
    Task UpdateMeasurementAsync(Guid projectId, Guid measurementId, MeasurementInput input);
    Task DeleteMeasurementAsync(Guid projectId, Guid measurementId);

    Task AddMachineSettingAsync(Guid projectId, MachineSettingInput input);
    Task UpdateMachineSettingAsync(Guid projectId, Guid machineSettingId, MachineSettingInput input);
    Task DeleteMachineSettingAsync(Guid projectId, Guid machineSettingId);
}
