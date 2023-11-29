using System;
namespace SIMS_Backend
{
	public class Notification
	{
		public int NotificationId { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public bool Notified { get; set; } = false;
		public int NotifyUID { get; set; }
		public DateTime MessageCreationTime { get; set; } = DateTime.Now;

		public Notification(string title, string message, int notifyUID) { this.Title = title; this.Message = message; this.NotifyUID = notifyUID; }

		public static Notification convertCreationObj(NotificationCreation creationObj)
		{
			return new Notification(creationObj.Title, creationObj.Message, creationObj.NotifyUID);

		}
	}
	public class NotificationCreation
	{
        public string Title { get; set; }
        public string Message { get; set; }
        public int NotifyUID { get; set; }
    }
	
}

