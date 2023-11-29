using System;
using SIMS_Frontend;
using Newtonsoft.Json;
using System.Net;
namespace SIMS_Frontend.Classes
{
	public class EmployeeService
	{
		
        public static EmployeeReturnObj GetEmployees(string authToken)
        {
            HttpResponseMessage message = Backend.GetBackendService("/Employee", authToken);
            if (message.StatusCode != HttpStatusCode.OK) { return new EmployeeReturnObj((int)message.StatusCode); };
             string jsonResult;
            using (var task = Task.Run(() => message.Content.ReadAsStringAsync()))
            {
                task.Wait();
                jsonResult = task.Result;
            }
            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(jsonResult);
            return new EmployeeReturnObj((int)message.StatusCode, employees);
        }
        public static Employee GetEmployee(int id, string authToken)
        {
            HttpResponseMessage message = Backend.GetBackendService("/Employee/"+ id.ToString(), authToken);
            if (message.StatusCode != HttpStatusCode.OK) { return new Employee(); };
            string jsonResult;
            using (var task = Task.Run(() => message.Content.ReadAsStringAsync()))
            {
                task.Wait();
                jsonResult = task.Result;
            }
            Employee employee = JsonConvert.DeserializeObject<Employee>(jsonResult) ;
            return employee;
        }


    }
    public class EmployeeReturnObj
    {
        public int Statuscode { get; set; }
        public string? Message { get; set; }
        public List<Employee>? Employees { get; set; }
        public EmployeeReturnObj() { }
        public EmployeeReturnObj(int statuscode) { this.Statuscode = statuscode; }

        public EmployeeReturnObj(int statuscode, List<Employee> employees) { this.Statuscode = statuscode; this.Employees = employees; }
    }
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int? Uid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Name { get { return this.LastName + " " + this.FirstName; } }
        public string? Mail { get; set; }
        public string? Phone { get; set; }
        public int? SuperiorID { get; set; }

    }
}

