using CompressKly.Application.DTOs;
using CompressKly.Application.Interfaces;
using CompressKly.Domain.Enums;
using CompressKly.Domain.Exceptions;

namespace CompressKly.Application.Videos.GetStatus;

public sealed class GetCompressionStatusHandler
{
    private readonly IVideoRepository _repository;

    public GetCompressionStatusHandler(IVideoRepository repository)
    {
        _repository = repository;
    }

    public async Task<CompressionStatusResponse> HandleAsync(
        GetCompressionStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var video = await _repository.GetByIdAsync(query.VideoId, cancellationToken)
            ?? throw new NotFoundException("Vídeo não encontrado.");

        var message = video.Status switch
        {
            CompressionStatus.Uploaded => "Vídeo enviado.",
            CompressionStatus.Queued => "Na fila de compressão...",
            CompressionStatus.Preparing => "Preparando vídeo...",
            CompressionStatus.Processing => "Processando...",
            CompressionStatus.Compressing => "Comprimindo...",
            CompressionStatus.Finalizing => "Finalizando...",
            CompressionStatus.Completed => "Compressão concluída",
            CompressionStatus.Failed => video.ErrorMessage ?? "Erro no processamento",
            _ => "Processando..."
        };

        string? downloadUrl = video.Status == CompressionStatus.Completed
            ? $"/api/videos/{video.Id}/download"
            : null;

        return new CompressionStatusResponse(
            video.Id,
            video.Status.ToString(),
            video.Progress,
            message,
            video.OriginalFileName,
            video.OriginalSize,
            video.CompressedSize,
            video.Status == CompressionStatus.Completed ? video.SavedBytes : null,
            video.Status == CompressionStatus.Completed ? video.ReductionPercent : null,
            downloadUrl,
            video.Status == CompressionStatus.Completed ? video.CompressedFileName : null,
            video.ErrorMessage);
    }
}
