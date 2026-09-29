namespace CompressKly.Domain.Enums;

public enum CompressionStatus
{
    Uploaded = 0,
    Queued = 1,
    Preparing = 2,
    Processing = 3,
    Compressing = 4,
    Finalizing = 5,
    Completed = 6,
    Failed = 7
}
