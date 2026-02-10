using System.Security.Cryptography;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Infrastructure.Repositories;

public class AppCredentialRepository : IAppCredentialRepository
{
    private readonly HappyDbContext _db;

    public AppCredentialRepository(HappyDbContext db)
    {
        _db = db;
    }

    public async Task<AppCredential?> ValidateAsync(string appSecret, string phone)
    {
        return await _db.AppCredentials
            .FirstOrDefaultAsync(x => x.AppSecret == appSecret && x.Phone == phone && x.IsActive);
    }

    public async Task<AppCredential?> GetByPhoneAndAppNameAsync(string phone, string appName)
    {
        return await _db.AppCredentials
            .FirstOrDefaultAsync(x => x.Phone == phone && x.AppName == appName);
    }

    public async Task<AppCredential> CreateAsync(string appName, string phone)
    {
        var appCredential = new AppCredential
        {
            AppSecret = GenerateSecretKey(),
            AppName = appName,
            Phone = phone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.AppCredentials.Add(appCredential);
        await _db.SaveChangesAsync();

        return appCredential;
    }

    public async Task<bool> ExistsByPhoneAndAppNameAsync(string phone, string appName)
    {
        return await _db.AppCredentials.AnyAsync(x => x.Phone == phone && x.AppName == appName);
    }

    public async Task<AppCredential?> GetLatestByPhoneAsync(string phone)
    {
        return await _db.AppCredentials
            .Where(x => x.Phone == phone)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var appCredential = await _db.AppCredentials.FindAsync(id);
        if (appCredential == null)
            return false;

        appCredential.IsActive = false;
        appCredential.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return true;
    }

    private static string GenerateSecretKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return $"sk_{Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "")}";
    }
}
