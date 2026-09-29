using CompressKly.Application.DTOs;
using CompressKly.Application.Interfaces;
using CompressKly.Domain.Enums;
using CompressKly.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompressKly.Application.Videos.Compress;

public sealed class CompressVideoHandler
{
    private readonly IVideoRepository _repository;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CompressVideoHandler> _logger;

    public CompressVideoHandler(
        IVideoRepository repository,
        IServiceScopeFactory scopeFactory,
        ILogger<CompressVideoHandler> logger)
    {
        _repository = repository;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<CompressVideoResponse> HandleAsync(
        CompressVideoCommand command,
        CancellationToken cancellationToken = default)
    {
        var video = await _repository.GetByIdAsync(command.VideoId, cancellationToken)
            ?? throw new NotFoundException("Vídeo não encontrado.");

        video.QueueCompression(command.Quality);
        await _repository.UpdateAsync(video, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var videoId = video.Id;
        _ = Task.Run(async () =>
        {
            try
            {
                await RunCompressionAsync(videoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha não tratada na compressão do vídeo {VideoId}", videoId);
            }
        }, CancellationToken.None);

        return new CompressVideoResponse(video.Id, video.Status.ToString(), video.Progress);
    }

    private async Task RunCompressionAsync(Guid videoId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
        var storage = scope.ServiceProvider.GetRequiredService<IVideoStorage>();
        var compressor = scope.ServiceProvider.GetRequiredService<IVideoCompressor>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CompressVideoHandler>>();

        var video = await repository.GetByIdAsync(videoId);
        if (video is null)
        {
            return;
        }

        try
        {
            if (!storage.OriginalExists(video.Id, video.SafeFileName))
            {
                throw new CompressionException("Arquivo original não encontrado.");
            }

            var inputPath = storage.GetOriginalPath(video.Id, video.SafeFileName);
            var outputPath = storage.GetCompressedPath(video.Id, video.CompressedFileName);

            video.MarkPreparing();
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();

            video.MarkProcessing();
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();

            video.MarkCompressing(20);
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();

            var lastPersistedProgress = video.Progress;
            await compressor.CompressAsync(
                inputPath,
                outputPath,
                video.Quality ?? CompressionQuality.Balanced,
                progress =>
                {
                    var mapped = Math.Max(20, Math.Min(92, 20 + (int)(progress * 0.72)));
                    if (mapped - lastPersistedProgress < 5 && mapped < 92)
                    {
                        return;
                    }

                    lastPersistedProgress = mapped;
                    video.MarkCompressing(mapped);
                    _ = PersistProgressAsync(videoId, mapped);
                });

            video.MarkFinalizing();
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();

            var compressedSize = storage.GetFileSize(outputPath);
            video.MarkCompleted(compressedSize);
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();

            await storage.DeleteOriginalAsync(video.Id, video.SafeFileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao comprimir vídeo {VideoId}", videoId);

            var message = ex is CompressionException
                ? ex.Message
                : "Não foi possível comprimir o vídeo. Tente novamente.";

            video.MarkFailed(message);
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();
        }
    }

    private async Task PersistProgressAsync(Guid videoId, int progress)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
            var video = await repository.GetByIdAsync(videoId);
            if (video is null)
            {
                return;
            }

            video.MarkCompressing(progress);
            await repository.UpdateAsync(video);
            await repository.SaveChangesAsync();
        }
        catch
        {
            // Ignore transient progress write failures.
        }
    }
}
