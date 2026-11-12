using CompressKly.Application.DTOs;
using CompressKly.Application.Interfaces;
using CompressKly.Core.Enums;
using CompressKly.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompressKly.Application.Videos.Download;

public sealed class DownloadVideoHandler
{
    private readonly IVideoRepository _repository;
    private readonly IVideoStorage _storage;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DownloadVideoHandler> _logger;

    public DownloadVideoHandler(
        IVideoRepository repository,
        IVideoStorage storage,
        IServiceScopeFactory scopeFactory,
        ILogger<DownloadVideoHandler> logger)
    {
        _repository = repository;
        _storage = storage;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<DownloadVideoResult> HandleAsync(
        DownloadVideoQuery query,
        CancellationToken cancellationToken = default)
    {
        var video = await _repository.GetByIdAsync(query.VideoId, cancellationToken)
            ?? throw new NotFoundException("Vídeo não encontrado.");

        if (video.Status != CompressionStatus.Completed || video.CompressedSize is null)
        {
            throw new NotFoundException("Arquivo comprimido não disponível.");
        }

        if (!_storage.CompressedExists(video.Id, video.CompressedFileName))
        {
            throw new NotFoundException("Arquivo comprimido não encontrado.");
        }

        var path = _storage.GetCompressedPath(video.Id, video.CompressedFileName);
        var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);

        if (query.CleanupAfter)
        {
            var videoId = video.Id;
            var safeName = video.SafeFileName;
            var compressedName = video.CompressedFileName;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
                    var storage = scope.ServiceProvider.GetRequiredService<IVideoStorage>();

                    var entity = await repository.GetByIdAsync(videoId);
                    if (entity is not null)
                    {
                        await storage.DeleteAllAsync(videoId, safeName, compressedName);
                        await repository.DeleteAsync(entity);
                        await repository.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao limpar vídeo {VideoId} após download", videoId);
                }
            });
        }

        return new DownloadVideoResult(
            stream,
            video.CompressedFileName,
            "video/mp4",
            video.CompressedSize.Value);
    }
}
