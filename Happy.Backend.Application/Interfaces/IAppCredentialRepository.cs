using Happy.Backend.Domain.Entities;

namespace Happy.Backend.Application.Interfaces;

public interface IAppCredentialRepository
{
    Task<AppCredential?> ValidateAsync(string appSecret, string phone);
    Task<AppCredential?> GetByPhoneAndAppNameAsync(string phone, string appName);
    Task<AppCredential> CreateAsync(string appName, string phone);
    Task<bool> ExistsByPhoneAndAppNameAsync(string phone, string appName);
    Task<bool> DeactivateAsync(int id);
}
