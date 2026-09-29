using CompressKly.Application.DTOs;

namespace CompressKly.Application.Videos.Upload;

public sealed class UploadVideoCommand
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required long Size { get; init; }
}
