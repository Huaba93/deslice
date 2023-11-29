using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS_Backend
{
	public class Issue
	{
		public int IssueID { get; set; }
		public string CVE { get; set; }
		public decimal CvssBaseScore { get; set; }
		public string? Cvss { get; set; }
		public string? Description { get; set; }
		public enum Status { Open = 0, InProgress = 1, Workaround = 2, Closed = 4 }
		public DateTime ReportedTime { get; set; }
		public DateTime? ClosedTime { get; set; }
		public int ReportedEmployeeID { get; set; }
		public Employee? ReportedEmployee { get { SIMSContext context = new(); return context.Employees.Find(ReportedEmployeeID); } }
		public int? AssignedEmployeeID { get; set; }
		public Employee? AssignedEmployee { get { SIMSContext context = new(); return context.Employees.Find(AssignedEmployeeID); } }
		public List<System>? AffectedSystems { get; } = new();
	}

	//public class IssueSystem
	//{
	//	public int IssueID { get; set; }
	//	public Issue Issue { get; set; } = null;
	//	public int SystemID { get; set; }
	//	public System System { get; set; } = null;
	//}

}
