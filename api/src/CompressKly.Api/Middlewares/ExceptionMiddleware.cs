using System.Net;
using System.Text.Json;
using CompressKly.Domain.Exceptions;
using FluentValidation;
using DomainValidationException = CompressKly.Domain.Exceptions.ValidationException;

namespace CompressKly.Api.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, payload) = exception switch
        {
            DomainValidationException validation => (
                HttpStatusCode.BadRequest,
                new ErrorResponse(validation.Message, validation.Errors)),
            FluentValidation.ValidationException fluent => (
                HttpStatusCode.BadRequest,
                new ErrorResponse(
                    fluent.Errors.FirstOrDefault()?.ErrorMessage ?? "Dados inválidos.",
                    fluent.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))),
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                new ErrorResponse(notFound.Message)),
            CompressionException compression => (
                HttpStatusCode.UnprocessableEntity,
                new ErrorResponse(compression.Message)),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse("Ocorreu um erro inesperado. Tente novamente."))
        };

        if ((int)statusCode >= 500)
        {
            _logger.LogError(exception, "Erro não tratado na API");
        }
        else
        {
            _logger.LogWarning(exception, "Erro de negócio na API");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private sealed record ErrorResponse(
        string Message,
        IReadOnlyDictionary<string, string[]>? Errors = null);
}
