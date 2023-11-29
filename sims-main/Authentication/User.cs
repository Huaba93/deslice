using System;
namespace SIMS_Authentication
{
	public class User 
	{
		public int UserID { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public byte[] Salt { get; set; }
		public int RoleID { get; set; }
		public Role? Role { get { AuthContext context = new(); return context.Roles.Find(this.RoleID); } }

		public string Name { get { return this.Firstname + " " + this.Lastname; } }
	}
	public class UserDTO
	{
		public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public int RoleID { get; set; }
    }
	public class CreateUserDTO : UserDTO
	{
        public string Password { get; set; }

    }
	public class Auth
	{
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";
	}
}

