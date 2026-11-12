using System.Net;
using CompressKly.Core.Models.ViewModels;

namespace CompressKly.Core.Interfaces.Notifications;

public interface INotifier
{
    void ClearNotifications();
    bool HasAnyNotification();
    List<NotificationViewModel> GetNotifications();
    void Handle(string message, HttpStatusCode statusCode);
}
