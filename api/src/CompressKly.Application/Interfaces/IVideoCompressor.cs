using CompressKly.Domain.Enums;

namespace CompressKly.Application.Interfaces;

public interface IVideoCompressor
{
    Task CompressAsync(
        string inputPath,
        string outputPath,
        CompressionQuality quality,
        Action<int>? onProgress = null,
        CancellationToken cancellationToken = default);
}
