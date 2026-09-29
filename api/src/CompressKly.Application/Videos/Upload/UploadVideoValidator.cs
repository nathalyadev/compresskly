using FluentValidation;

namespace CompressKly.Application.Videos.Upload;

public sealed class UploadVideoValidator : AbstractValidator<UploadVideoCommand>
{
    public const long DefaultMaxFileSizeBytes = 500L * 1024 * 1024;

    private static readonly string[] AllowedContentTypes =
    [
        "video/mp4",
        "video/x-m4v",
        "application/mp4",
        "application/octet-stream"
    ];

    public UploadVideoValidator(long? maxFileSizeBytes = null)
    {
        var maxBytes = maxFileSizeBytes ?? DefaultMaxFileSizeBytes;

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(name => name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            .WithMessage("O arquivo selecionado não é um vídeo MP4 válido.");

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithMessage("O arquivo selecionado não é um vídeo MP4 válido.")
            .LessThanOrEqualTo(maxBytes)
            .WithMessage($"O arquivo excede o tamanho máximo permitido ({maxBytes / (1024 * 1024)} MB).");

        RuleFor(x => x.ContentType)
            .Must(contentType =>
                string.IsNullOrWhiteSpace(contentType) ||
                AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage("O arquivo selecionado não é um vídeo MP4 válido.");

        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("O arquivo de vídeo é obrigatório.");
    }
}
