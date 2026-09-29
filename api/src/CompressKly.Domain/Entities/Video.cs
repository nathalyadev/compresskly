using CompressKly.Domain.Enums;
using CompressKly.Domain.Exceptions;

namespace CompressKly.Domain.Entities;

public sealed class Video
{
    public Guid Id { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string SafeFileName { get; private set; } = string.Empty;
    public string CompressedFileName { get; private set; } = string.Empty;
    public long OriginalSize { get; private set; }
    public long? CompressedSize { get; private set; }
    public CompressionQuality? Quality { get; private set; }
    public CompressionStatus Status { get; private set; }
    public int Progress { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private Video()
    {
    }

    public static Video Create(string originalFileName, string safeFileName, long originalSize)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ValidationException("O nome do arquivo é obrigatório.");
        }

        if (originalSize <= 0)
        {
            throw new ValidationException("O arquivo de vídeo é inválido.");
        }

        var id = Guid.NewGuid();
        var compressedFileName = BuildCompressedFileName(safeFileName);

        return new Video
        {
            Id = id,
            OriginalFileName = originalFileName.Trim(),
            SafeFileName = safeFileName,
            CompressedFileName = compressedFileName,
            OriginalSize = originalSize,
            Status = CompressionStatus.Uploaded,
            Progress = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void QueueCompression(CompressionQuality quality)
    {
        if (Status is CompressionStatus.Compressing or CompressionStatus.Queued)
        {
            throw new ValidationException("A compressão deste vídeo já está em andamento.");
        }

        if (Status == CompressionStatus.Completed)
        {
            throw new ValidationException("Este vídeo já foi comprimido.");
        }

        Quality = quality;
        Status = CompressionStatus.Queued;
        Progress = 5;
        ErrorMessage = null;
        CompressedSize = null;
        CompletedAt = null;
    }

    public void MarkPreparing()
    {
        Status = CompressionStatus.Preparing;
        Progress = Math.Max(Progress, 10);
    }

    public void MarkProcessing()
    {
        Status = CompressionStatus.Processing;
        Progress = Math.Max(Progress, 15);
    }

    public void MarkCompressing(int progress)
    {
        Status = CompressionStatus.Compressing;
        Progress = Math.Clamp(progress, 15, 92);
    }

    public void MarkFinalizing()
    {
        Status = CompressionStatus.Finalizing;
        Progress = 95;
    }

    public void MarkCompleted(long compressedSize)
    {
        if (compressedSize <= 0)
        {
            throw new CompressionException("Falha ao gerar o arquivo final.");
        }

        CompressedSize = compressedSize;
        Status = CompressionStatus.Completed;
        Progress = 100;
        ErrorMessage = null;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed(string message)
    {
        Status = CompressionStatus.Failed;
        ErrorMessage = message;
        Progress = 0;
    }

    public long SavedBytes =>
        CompressedSize is null ? 0 : Math.Max(OriginalSize - CompressedSize.Value, 0);

    public double ReductionPercent =>
        OriginalSize <= 0 || CompressedSize is null
            ? 0
            : SavedBytes / (double)OriginalSize * 100d;

    private static string BuildCompressedFileName(string safeFileName)
    {
        var stem = safeFileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
            ? safeFileName[..^4]
            : safeFileName;

        return $"{stem}-comprimido.mp4";
    }
}
