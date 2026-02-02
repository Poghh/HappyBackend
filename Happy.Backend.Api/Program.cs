using Happy.Backend.Api.Models;
using Happy.Backend.Api.Services;
using Happy.Backend.Application.Interfaces;
using Happy.Backend.Infrastructure.Data;
using Happy.Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtService, JwtService>();

// DbContext PostgreSQL
builder.Services.AddDbContext<HappyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ISyncRawRepository, SyncRawRepository>();
builder.Services.AddScoped<IAppCredentialRepository, AppCredentialRepository>();

var app = builder.Build();

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
