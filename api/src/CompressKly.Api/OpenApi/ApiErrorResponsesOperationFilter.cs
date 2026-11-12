using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CompressKly.Api.OpenApi;

/// <summary>
/// Garante que cada operação documente o corpo padrão <see cref="ApiErrorResponse"/> nos status de erro comuns.
/// </summary>
public sealed class ApiErrorResponsesOperationFilter : IOperationFilter
{
    private static readonly (string Code, string Description)[] DefaultErrors =
    [
        ("400", "Requisição inválida ou falha de validação."),
        ("404", "Recurso não encontrado."),
        ("422", "Falha ao processar o recurso."),
        ("500", "Erro interno do servidor.")
    ];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses ??= new OpenApiResponses();

        var errorSchema = context.SchemaGenerator.GenerateSchema(
            typeof(ApiErrorResponse),
            context.SchemaRepository);

        foreach (var (code, defaultDescription) in DefaultErrors)
        {
            operation.Responses.TryGetValue(code, out var existing);

            var description = existing?.Description;
            if (string.IsNullOrWhiteSpace(description))
            {
                description = defaultDescription;
            }

            var hasJsonBody = existing?.Content?.ContainsKey("application/json") == true;

            operation.Responses[code] = new OpenApiResponse
            {
                Description = description,
                Content = hasJsonBody
                    ? CloneContent(existing!.Content!)
                    : new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType { Schema = errorSchema }
                    }
            };
        }
    }

    private static Dictionary<string, OpenApiMediaType> CloneContent(
        IDictionary<string, OpenApiMediaType> content)
    {
        var clone = new Dictionary<string, OpenApiMediaType>();
        foreach (var (key, media) in content)
        {
            clone[key] = new OpenApiMediaType(media);
        }

        return clone;
    }
}
