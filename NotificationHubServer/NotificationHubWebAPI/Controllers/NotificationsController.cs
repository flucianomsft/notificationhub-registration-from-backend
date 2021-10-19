using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationHubWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(string pns, [FromBody] string message, string toUser)
        {
            string[] userTag = new string[1];
            userTag[0] = "username:" + toUser;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;

            switch (pns.ToLower())
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                "From " + toUser + ": " + message + "</text></binding></visual></toast>";
                    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                    break;
                case "apns":
                    // iOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + toUser + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "fcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + "From " + toUser + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendFcmNativeNotificationAsync(notif, userTag);
                    break;
            }
            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    return Ok();
                }
            }
            return StatusCode(500);
        }

        [HttpPost("template")]
        public async Task<IActionResult> PostTemplate(string toUser)
        {
            var userTag = "username:" + toUser;

            var notification = new Dictionary<string, string> { { "message", "Hello, " + toUser } };
            await Notifications.Instance.Hub.SendTemplateNotificationAsync(notification, userTag);

            return Ok();
        }


    }
}
