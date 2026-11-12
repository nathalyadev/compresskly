using CompressKly.Core.Enums;

namespace CompressKly.Application.Videos.Compress;

public sealed class CompressVideoCommand
{
    public required Guid VideoId { get; init; }
    public required CompressionQuality Quality { get; init; }
}
