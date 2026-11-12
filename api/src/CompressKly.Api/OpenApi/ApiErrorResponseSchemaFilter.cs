using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CompressKly.Api.OpenApi;

public sealed class ApiErrorResponseSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(ApiErrorResponse) || schema is not OpenApiSchema openApiSchema)
        {
            return;
        }

        openApiSchema.Description =
            "Erro retornado pela API. O HTTP status indica a categoria; `message` traz o resumo e `errors` lista falhas por campo quando houver validação.";

        openApiSchema.Example = JsonNode.Parse(
            """
            {
              "message": "O arquivo de vídeo é obrigatório.",
              "errors": {
                "video": ["O arquivo de vídeo é obrigatório."]
              }
            }
            """);
    }
}
