using System;
using Microsoft.AspNetCore.Mvc;

namespace SIMS_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase

    {
        [HttpPost(Name = "PostNotification")]
        public IActionResult CreateNotification([FromBody] NotificationCreation notification, [FromHeader] string authToken)
        {
            AuthObj auth = Authentication.VerifyJWT(authToken);
            if (auth.tokenValid && Authentication.isAdmin(auth))
            {
                using SIMSContext context = new();
                context.Notifications.Add(Notification.convertCreationObj(notification));
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet(Name = "GetNotifications")]
        public IActionResult GetNotifications([FromHeader] string authToken)
        {
            AuthObj auth = Authentication.VerifyJWT(authToken);
            if (auth.tokenValid && Authentication.isAdmin(auth))
            {
                using SIMSContext context = new();
                List<Notification> notifications = context.Notifications.ToList();
                return Ok(notifications);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("{uid}", Name = "GetNotificationForUser")]
        public IActionResult GetNotificationsForUser(int uid, [FromHeader] string authToken)
        {
            AuthObj auth = Authentication.VerifyJWT(authToken);
            if (auth.tokenValid && Authentication.isAdmin(auth))
            {
                using SIMSContext context = new();
                List<Notification> notifications = context.Notifications.Where(n => n.NotifyUID == uid).ToList();
                return Ok(notifications);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut(Name = "MarkNotificationNotified")]
        public IActionResult MarkNotification([FromQuery] int id, [FromHeader] string authToken)
        {
            AuthObj auth = Authentication.VerifyJWT(authToken);
            if (auth.tokenValid && Authentication.isAdmin(auth))
            {
                using SIMSContext context = new();
                Notification? notification = context.Notifications.Find(id);
                if (notification is null)
                {
                    return NotFound("Notification not found");
                }
                else
                {
                    notification.Notified = true;
                    context.Notifications.Update(notification);
                    context.SaveChanges();
                    return Ok("Notification marked as notified");
                }
            }
            else
            {
                return Unauthorized();

            }
        }

    }
}

