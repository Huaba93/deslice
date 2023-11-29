using System;

namespace SIMS_Backend
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string UID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public int? SuperiorID { get; set; }
    }
}