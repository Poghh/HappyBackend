using Happy.Backend.Application.Interfaces;
using Happy.Backend.Application.Services;
using Happy.Backend.Infrastructure.Data;
using Happy.Backend.Infrastructure.Repositories;
using Happy.Backend.Infrastructure.Services;
using Happy.Backend.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<MinioSettings>(configuration.GetSection("Minio"));

        // Infrastructure Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IObjectStorageService, MinioStorageService>();

        // DbContext PostgreSQL
        services.AddDbContext<HappyDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISyncRawRepository, SyncRawRepository>();
        services.AddScoped<IAppCredentialRepository, AppCredentialRepository>();
        services.AddScoped<IStoreFarmerInformationRepository, StoreFarmerInformationRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IStockInRepository, StockInRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IHarvestRepository, HarvestRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFarmerService, FarmerService>();
        services.AddScoped<IStoreService, StoreService>();

        return services;
    }
}
