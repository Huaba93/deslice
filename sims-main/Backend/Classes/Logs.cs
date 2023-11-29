using System;
namespace SIMS_Backend
{
	public class Logs
	{
		public int ID { get; set; }
		public DateTime Created { get; set; }= DateTime.Now;
		public int Severity { get; set; } = 3; // 1 = low, 2= medium, 3= high, 4=debug
		public string Source { get; set; } = "Backend";
		public string? UserID { get; set; } 
		public string Logtext { get; set; } = "";
		public Logs() { }
		public Logs(int Severity, string Source, string UserID, string Logtext) {
			this.Severity = Severity; this.Source = Source; this.UserID = UserID; this.Logtext = Logtext;
		}
		public Logs(int Severity, string Source, string Logtext)
		{
			this.Severity = Severity; this.Source = Source; this.Logtext = Logtext;
		}
		public Logs(string Logtext)
		{
			this.Logtext = Logtext;
		}
	}
	public class Logservice
	{
		public static void  writeLog(int? Severity, string? Source, string? UserID, string Logtext)
        {

			Logs logentry = new(Logtext);
			if (Severity.HasValue) { logentry.Severity = Severity.Value; }
			if (null != Source) { logentry.Source = Source; }
			if (null != UserID) { logentry.UserID = UserID; }
			
			SIMSContext context = new();
			context.Logs.Add(logentry);
			context.SaveChanges();

        }
    }


}

