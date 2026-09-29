using CompressKly.Application.Videos.Compress;
using CompressKly.Application.Videos.Download;
using CompressKly.Application.Videos.GetStatus;
using CompressKly.Application.Videos.Upload;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CompressKly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, long? maxFileSizeBytes = null)
    {
        services.AddScoped<IValidator<UploadVideoCommand>>(_ =>
            new UploadVideoValidator(maxFileSizeBytes));

        services.AddScoped<UploadVideoHandler>();
        services.AddScoped<CompressVideoHandler>();
        services.AddScoped<GetCompressionStatusHandler>();
        services.AddScoped<DownloadVideoHandler>();

        return services;
    }
}
