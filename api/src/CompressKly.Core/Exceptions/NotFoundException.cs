namespace CompressKly.Core.Exceptions;

public sealed class NotFoundException : CoreException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
