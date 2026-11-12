using System.Net;
using CompressKly.Core.Interfaces.Notifications;
using CompressKly.Core.Models.ViewModels;

namespace CompressKly.Application.Notifications;

public class Notifier : INotifier
{
    private readonly List<NotificationViewModel> _notifications;

    public Notifier() => _notifications = [];

    public void Handle(string message, HttpStatusCode statusCode)
    {
        NotificationViewModel notification = new(message, statusCode);

        _notifications.Add(notification);
    }

    public void ClearNotifications()
    {
        if (_notifications.Count > 0)
            _notifications.Clear();
    }

    public List<NotificationViewModel> GetNotifications() => _notifications;

    public bool HasAnyNotification() => _notifications.Count > 0;
}
