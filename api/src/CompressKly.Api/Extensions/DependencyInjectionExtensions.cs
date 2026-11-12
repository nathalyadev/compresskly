using CompressKly.Application;
using CompressKly.Core;
using CompressKly.Infrastructure;

namespace CompressKly.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCompressKlyServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var maxFileSizeMb = configuration.GetValue("Upload:MaxFileSizeMb", 500);
        var maxFileSizeBytes = maxFileSizeMb * 1024L * 1024L;

        services.AddApplication(maxFileSizeBytes);
        services.AddApplicationModule();
        services.AddCoreModule();
        services.AddInfrastructure(configuration);

        services.Configure<FormOptionsSetup>(configuration.GetSection("Upload"));

        return services;
    }
}

public sealed class FormOptionsSetup
{
    public int MaxFileSizeMb { get; set; } = 500;
}
