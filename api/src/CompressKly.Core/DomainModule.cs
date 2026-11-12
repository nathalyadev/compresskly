using Microsoft.Extensions.DependencyInjection;

namespace CompressKly.Core;

public static class CoreModule
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services)
    {
        services.AddCoreServices();

        return services;    
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services;
    }
}
