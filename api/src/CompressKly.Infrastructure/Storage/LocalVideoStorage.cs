using CompressKly.Application.Interfaces;
using CompressKly.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace CompressKly.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string RootPath { get; set; } = "storage";
    public string OriginalFolder { get; set; } = "original";
    public string CompressedFolder { get; set; } = "compressed";
}

public sealed class LocalVideoStorage : IVideoStorage
{
    private readonly string _originalRoot;
    private readonly string _compressedRoot;

    public LocalVideoStorage(IOptions<StorageOptions> options)
    {
        var settings = options.Value;
        var root = Path.GetFullPath(settings.RootPath);
        _originalRoot = Path.Combine(root, settings.OriginalFolder);
        _compressedRoot = Path.Combine(root, settings.CompressedFolder);

        Directory.CreateDirectory(_originalRoot);
        Directory.CreateDirectory(_compressedRoot);
    }

    public async Task<string> SaveOriginalAsync(
        Guid videoId,
        string safeFileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(_originalRoot, videoId.ToString("N"));
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, safeFileName);
        await using var fileStream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 64 * 1024,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);

        await content.CopyToAsync(fileStream, cancellationToken);
        return path;
    }

    public string GetOriginalPath(Guid videoId, string safeFileName) =>
        Path.Combine(_originalRoot, videoId.ToString("N"), safeFileName);

    public string GetCompressedPath(Guid videoId, string compressedFileName) =>
        Path.Combine(_compressedRoot, videoId.ToString("N"), compressedFileName);

    public Task DeleteOriginalAsync(Guid videoId, string safeFileName, CancellationToken cancellationToken = default)
    {
        DeleteDirectoryIfExists(Path.Combine(_originalRoot, videoId.ToString("N")));
        return Task.CompletedTask;
    }

    public Task DeleteCompressedAsync(Guid videoId, string compressedFileName, CancellationToken cancellationToken = default)
    {
        DeleteDirectoryIfExists(Path.Combine(_compressedRoot, videoId.ToString("N")));
        return Task.CompletedTask;
    }

    public Task DeleteAllAsync(
        Guid videoId,
        string safeFileName,
        string compressedFileName,
        CancellationToken cancellationToken = default)
    {
        DeleteDirectoryIfExists(Path.Combine(_originalRoot, videoId.ToString("N")));
        DeleteDirectoryIfExists(Path.Combine(_compressedRoot, videoId.ToString("N")));
        return Task.CompletedTask;
    }

    public bool OriginalExists(Guid videoId, string safeFileName) =>
        File.Exists(GetOriginalPath(videoId, safeFileName));

    public bool CompressedExists(Guid videoId, string compressedFileName) =>
        File.Exists(GetCompressedPath(videoId, compressedFileName));

    public long GetFileSize(string path)
    {
        if (!File.Exists(path))
        {
            throw new CompressionException("Arquivo de saída não encontrado.");
        }

        return new FileInfo(path).Length;
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
