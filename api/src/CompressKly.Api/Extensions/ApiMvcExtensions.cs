using CompressKly.Api.OpenApi;
using Microsoft.AspNetCore.Mvc;

namespace CompressKly.Api.Extensions;

public static class ApiMvcExtensions
{
    public static IMvcBuilder AddCompressKlyControllers(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
                new BadRequestObjectResult(ApiErrorResponse.FromModelState(context.ModelState));
        });

        return services
            .AddControllers()
            .AddJsonOptions(options => ApiJsonOptions.Configure(options.JsonSerializerOptions));
    }
}
