using System;
using Newtonsoft.Json;
using System.Net;

namespace SIMS_Frontend.Classes
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool Notified { get; set; } = false;
        public int NotifyUID { get; set; }
        public DateTime MessageCreationTime { get; set; } = DateTime.Now;

        public Notification() { }
        public Notification(string title, string message, int notifyUID) { this.Title = title; this.Message = message; this.NotifyUID = notifyUID; }

    }

    public class NotificationService
    {
        public static List<Notification> GetNotificationsForUser(int id, string authToken)
        {
            HttpResponseMessage message = Backend.GetBackendService("/Notification/" + id.ToString(), authToken);
            if (message.StatusCode != HttpStatusCode.OK) { return new List<Notification>(); };
            string jsonResult;
            using (var task = Task.Run(() => message.Content.ReadAsStringAsync()))
            {
                task.Wait();
                jsonResult = task.Result;
            }
            List<Notification> notifications = JsonConvert.DeserializeObject<List<Notification>>(jsonResult);
            return notifications;
        }
    }
}

