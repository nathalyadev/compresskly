using System.Reflection;
using CompressKly.Api.OpenApi;
using Microsoft.OpenApi;

namespace CompressKly.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCompressKlyOpenApi(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "CompressKly API",
                    Version = "v1",
                    Description = "API REST para upload, compressão e download de vídeos MP4.",
                    Contact = new OpenApiContact
                    {
                        Name = "CompressKly"
                    }
                });

            options.TagActionsBy(_ => ["Videos"]);
            options.ResolveConflictingActions(descriptions => descriptions.First());

            IncludeXmlComments(options, Assembly.GetExecutingAssembly());
            IncludeXmlComments(options, typeof(CompressKly.Application.DTOs.UploadVideoResponse).Assembly);

            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);
            options.SchemaFilter<ApiErrorResponseSchemaFilter>();
            options.OperationFilter<ApiErrorResponsesOperationFilter>();
        });

        return services;
    }

    public static WebApplication UseCompressKlyOpenApi(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "CompressKly API v1");
            options.DocumentTitle = "CompressKly API";
            options.DisplayRequestDuration();
        });

        return app;
    }

    private static void IncludeXmlComments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        Assembly assembly)
    {
        var fileName = $"{assembly.GetName().Name}.xml";
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        if (File.Exists(path))
        {
            options.IncludeXmlComments(path, includeControllerXmlComments: true);
        }
    }
}
