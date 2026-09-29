namespace CompressKly.Application.Videos.Download;

public sealed class DownloadVideoQuery
{
    public required Guid VideoId { get; init; }
    public bool AsAttachment { get; init; } = true;
    public bool CleanupAfter { get; init; } = true;
}
