namespace CompressKly.Application.Interfaces;

public interface IVideoStorage
{
    Task<string> SaveOriginalAsync(
        Guid videoId,
        string safeFileName,
        Stream content,
        CancellationToken cancellationToken = default);

    string GetOriginalPath(Guid videoId, string safeFileName);

    string GetCompressedPath(Guid videoId, string compressedFileName);

    Task DeleteOriginalAsync(Guid videoId, string safeFileName, CancellationToken cancellationToken = default);

    Task DeleteCompressedAsync(Guid videoId, string compressedFileName, CancellationToken cancellationToken = default);

    Task DeleteAllAsync(Guid videoId, string safeFileName, string compressedFileName, CancellationToken cancellationToken = default);

    bool OriginalExists(Guid videoId, string safeFileName);

    bool CompressedExists(Guid videoId, string compressedFileName);

    long GetFileSize(string path);
}
