using System.Net;

namespace CompressKly.Core.Models.ViewModels;

public sealed class NotificationViewModel(string message, HttpStatusCode statusCode)
{
    public string Message { get; } = message;
    public HttpStatusCode StatusCode { get; } = statusCode;
}
