using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CompressKly.Api.OpenApi;

/// <summary>
/// Contrato padrão de erro da API. Todas as respostas 4xx e 5xx em <c>application/json</c> usam este formato.
/// </summary>
/// <param name="Message">Mensagem principal, legível para o usuário.</param>
/// <param name="Errors">
/// Mapa opcional de campo → mensagens, presente em falhas de validação (model binding, FluentValidation ou domínio).
/// </param>
public sealed record ApiErrorResponse(
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors = null)
{
    /// <summary>
    /// Monta o erro a partir do <see cref="ModelStateDictionary"/> do ASP.NET Core.
    /// </summary>
    public static ApiErrorResponse FromModelState(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Valor inválido."
                        : error.ErrorMessage)
                    .ToArray());

        var message = errors.Values.SelectMany(v => v).FirstOrDefault() ?? "Dados inválidos.";

        return new ApiErrorResponse(message, errors.Count > 0 ? errors : null);
    }

    /// <summary>
    /// Cria resposta de erro com mensagem única (sem detalhes por campo).
    /// </summary>
    public static ApiErrorResponse FromMessage(string message) => new(message);
}
