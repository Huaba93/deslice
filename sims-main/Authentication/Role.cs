using System;
namespace SIMS_Authentication
{
	public class Role
	{
		public int RoleID {get;set;}
		public string Name { get; set; }
		public int Perm { get; set; }

		[Flags]
		public enum Permissions : int
		{
			None = 0,
			Read = 1,
			Write = 2,
			Assign = 4
		};
	}
	
}

