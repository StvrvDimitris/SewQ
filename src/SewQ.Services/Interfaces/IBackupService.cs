using SewQ.Services.Models;

namespace SewQ.Services.Interfaces;

public interface IBackupService
{
    Task<BackupPayload> CreateBackupAsync();
}
