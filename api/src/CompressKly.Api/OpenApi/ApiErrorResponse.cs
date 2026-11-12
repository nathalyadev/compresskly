namespace CompressKly.Api.OpenApi;

/// <summary>
/// Corpo JSON retornado em erros tratados pela API (4xx e 5xx).
/// </summary>
/// <param name="Message">Mensagem principal do erro.</param>
/// <param name="Errors">Detalhes por campo, quando a validação falha.</param>
public sealed record ApiErrorResponse(
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors = null);
