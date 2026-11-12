namespace CompressKly.Core.Exceptions;

public sealed class CompressionException : CoreException
{
    public CompressionException(string message) : base(message)
    {
    }

    public CompressionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
