using CompressKly.Infrastructure.Data;
using CompressKly.Infrastructure.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CompressKly.IntegrationTests;

public sealed class CompressKlyWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"compresskly-tests-{Guid.NewGuid():N}.db");
    private readonly string _storageRoot = Path.Combine(Path.GetTempPath(), $"compresskly-storage-{Guid.NewGuid():N}");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_storageRoot);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<CompressKlyDbContext>));
            services.RemoveAll(typeof(CompressKlyDbContext));

            services.AddDbContext<CompressKlyDbContext>(options =>
                options.UseSqlite($"Data Source={_dbPath}"));

            services.PostConfigure<StorageOptions>(options =>
            {
                options.RootPath = _storageRoot;
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        try
        {
            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
            }

            if (Directory.Exists(_storageRoot))
            {
                Directory.Delete(_storageRoot, recursive: true);
            }
        }
        catch
        {
            // ignore cleanup errors in tests
        }
    }
}
