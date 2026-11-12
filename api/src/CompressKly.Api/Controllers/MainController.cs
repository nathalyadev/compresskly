using Microsoft.AspNetCore.Mvc;
using CompressKly.Core.Models.ViewModels;
using CompressKly.Core.Interfaces.Notifications;

namespace CompressKly.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(DefaultResponseViewModel), 500)]
public class MainController(INotifier notifier) : ControllerBase
{
    protected readonly INotifier _notifier = notifier;

    protected ActionResult PersonalizedResponse(ActionResult actionResult)
    {
        if (IsValidOperation())
            return actionResult;

        var notifications = _notifier.GetNotifications();

        return new JsonResult(new DefaultResponseViewModel(notifications.Select(n => n.Message)))
        {
            StatusCode = (int)notifications[0].StatusCode,
        };
    }

    private bool IsValidOperation() => !_notifier.HasAnyNotification();

}
