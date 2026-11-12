using CompressKly.Application.Notifications;
using CompressKly.Core.Interfaces.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace CompressKly.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services.AddNotifications();

        return services;
    }

    private static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddScoped<INotifier, Notifier>();

        return services;
    }
}
