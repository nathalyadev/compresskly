using CompressKly.Domain.Enums;

namespace CompressKly.Application.DTOs;

public sealed record UploadVideoResponse(
    Guid Id,
    string OriginalFileName,
    long OriginalSize,
    string Status);

public sealed record CompressVideoResponse(
    Guid Id,
    string Status,
    int Progress);

public sealed record CompressionStatusResponse(
    Guid Id,
    string Status,
    int Progress,
    string? Message,
    string OriginalFileName,
    long OriginalSize,
    long? CompressedSize,
    long? SavedBytes,
    double? ReductionPercent,
    string? DownloadUrl,
    string? FileName,
    string? ErrorMessage);

public sealed record DownloadVideoResult(
    Stream Content,
    string FileName,
    string ContentType,
    long ContentLength);
