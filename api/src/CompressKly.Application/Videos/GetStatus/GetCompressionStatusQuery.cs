namespace CompressKly.Application.Videos.GetStatus;

public sealed class GetCompressionStatusQuery
{
    public required Guid VideoId { get; init; }
}
