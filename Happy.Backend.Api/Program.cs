using Happy.Backend.Api.Constants;
using Happy.Backend.Api.Models;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Application.Services;
using Happy.Backend.Infrastructure.Data;
using Happy.Backend.Infrastructure.Repositories;
using Happy.Backend.Infrastructure.Services;
using Happy.Backend.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var firstError = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Invalid request";

            return new BadRequestObjectResult(new CommonResponseModel<object>(
                CommonResponseConstants.StatusBadRequest,
                null,
                firstError));
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// Infrastructure Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));

// Infrastructure Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<IObjectStorageService, MinioStorageService>();

// DbContext PostgreSQL
builder.Services.AddDbContext<HappyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositories
builder.Services.AddScoped<ISyncRawRepository, SyncRawRepository>();
builder.Services.AddScoped<IAppCredentialRepository, AppCredentialRepository>();
builder.Services.AddScoped<IStoreFarmerInformationRepository, StoreFarmerInformationRepository>();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFarmerService, FarmerService>();
builder.Services.AddScoped<IStoreService, StoreService>();

var app = builder.Build();

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HappyDbContext>();
    db.Database.Migrate();
}

// Middleware
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map controller routes
app.MapControllers();

app.Run();
