using SewQ.Services.Models;

namespace SewQ.Services.Interfaces;

public interface ISettingsService
{
    Task<AppSettingsDto> GetAsync();
    Task SaveAsync(AppSettingsDto settings);
}
