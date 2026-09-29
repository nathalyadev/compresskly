using CompressKly.Application.Interfaces;
using CompressKly.Infrastructure.Compression;
using CompressKly.Infrastructure.Data;
using CompressKly.Infrastructure.Repositories;
using CompressKly.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompressKly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.Configure<FfmpegOptions>(configuration.GetSection(FfmpegOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Default")
            ?? "Data Source=compresskly.db";

        services.AddDbContext<CompressKlyDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddSingleton<IVideoStorage, LocalVideoStorage>();
        services.AddSingleton<IVideoCompressor, FfmpegVideoCompressor>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CompressKlyDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
