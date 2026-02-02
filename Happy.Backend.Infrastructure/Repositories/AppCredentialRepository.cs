using System.Security.Cryptography;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Domain.Entities;
using Happy.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Infrastructure.Repositories;

public class AppCredentialRepository : IAppCredentialRepository
{
    private readonly HappyDbContext _context;

    public AppCredentialRepository(HappyDbContext context)
    {
        _context = context;
    }

    public async Task<AppCredential?> ValidateAsync(string appSecret, string phone)
    {
        return await _context.AppCredentials
            .FirstOrDefaultAsync(x => x.AppSecret == appSecret && x.Phone == phone && x.IsActive);
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

        _context.AppCredentials.Add(appCredential);
        await _context.SaveChangesAsync();

        return appCredential;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var appCredential = await _context.AppCredentials.FindAsync(id);
        if (appCredential == null)
            return false;

        appCredential.IsActive = false;
        appCredential.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    private static string GenerateSecretKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return $"sk_{Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "")}";
    }
}
