using System;
namespace SIMS_Backend
{
	public class System
	{
		public int SystemId { get; set; }
		public string Hostname { get; set; }
		public int ManufactorID { get; set; }
		public Manufactor? Manufactor { get { SIMSContext context = new(); return context.Manufactors.Find(ManufactorID); } }
		public int SystemtypeID { get; set; }
		public Systemtype? Systemtype { get { SIMSContext context = new(); return context.Systemtypes.Find(SystemtypeID); } }
		public string? SerialNumber { get; set; }
		public string? IpAddress { get; set; }
		public string? Location { get; set; }
		public int? Criticality { get; set; }
		//public ICollection<Issue>? Issues { get; set; }

	}

	public class Systemtype
	{
		public int SystemTypeId { get; set; }
		public string Name { get; set; }
	}
}

