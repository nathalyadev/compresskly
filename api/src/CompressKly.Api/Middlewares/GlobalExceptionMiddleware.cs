using System.Net;
using System.Text.Json;
using CompressKly.Api.OpenApi;
using CompressKly.Core.Exceptions;
using FluentValidation;
using CoreValidationException = CompressKly.Core.Exceptions.ValidationException;

namespace CompressKly.Api.Middlewares;

public sealed class GlobalExceptionMiddleware(
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment) : IMiddleware
{
    private const string DefaultErrorMessage =
        "Ocorreu um erro inesperado. Tente novamente.";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, payload) = MapException(exception);

        if ((int)statusCode >= 500)
        {
            logger.LogError(exception, "Erro não tratado na API");
        }
        else
        {
            logger.LogWarning(exception, "Erro de negócio na API");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, ApiJsonOptions.Create()));
    }

    private (HttpStatusCode StatusCode, ApiErrorResponse Payload) MapException(Exception exception)
    {
        return exception switch
        {
            CoreValidationException validation => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    validation.Message,
                    validation.Errors.Count > 0 ? validation.Errors : null)),
            FluentValidation.ValidationException fluent => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    fluent.Errors.FirstOrDefault()?.ErrorMessage ?? "Dados inválidos.",
                    fluent.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))),
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                ApiErrorResponse.FromMessage(notFound.Message)),
            CompressionException compression => (
                HttpStatusCode.UnprocessableEntity,
                ApiErrorResponse.FromMessage(compression.Message)),
            CoreException core => (
                HttpStatusCode.BadRequest,
                ApiErrorResponse.FromMessage(core.Message)),
            _ => (
                HttpStatusCode.InternalServerError,
                ApiErrorResponse.FromMessage(ResolveUnexpectedMessage(exception)))
        };
    }

    private string ResolveUnexpectedMessage(Exception exception)
    {
        if (!environment.IsDevelopment())
        {
            return DefaultErrorMessage;
        }

        return exception.InnerException?.Message
               ?? exception.Message
               ?? DefaultErrorMessage;
    }

}
