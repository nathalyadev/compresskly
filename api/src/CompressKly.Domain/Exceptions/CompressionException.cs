namespace CompressKly.Domain.Exceptions;

public sealed class CompressionException : DomainException
{
    public CompressionException(string message) : base(message)
    {
    }

    public CompressionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
