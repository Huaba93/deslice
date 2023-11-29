using System;
using SIMS_Frontend;
using Newtonsoft.Json;
using System.Net;

namespace SIMS_Frontend.Classes

{
    public class IssueService
    {



        public static IssueReturnObj GetIssues(string authToken)
        {
            IssueReturnObj retObj = new();
            string issueJson = Backend.GetJsonFromBackend("/Issue", authToken);
            if (int.TryParse(issueJson, out int statuscode)) { return new IssueReturnObj(statuscode); }
            List<Issue> issues = JsonConvert.DeserializeObject<List<Issue>>(issueJson) ?? new();
            retObj.Issue = issues;
            retObj.Employees = EmployeeService.GetEmployees(authToken).Employees;
            if (retObj.Employees != null)
            {
                foreach (Issue issue in issues)
                {
                    issue.ReportedEmployee = retObj.Employees.Find(e => e.EmployeeID == issue.ReportedEmployeeID) ?? new();
                    issue.AssignedEmployee = retObj.Employees.Find(e => e.EmployeeID == issue.AssignedEmployeeID) ?? new();
                }
            }
            return retObj;
        }
        public static ResponseObj PostIssue(string authToken,Issue issue)
        {
            string jsonObject = JsonConvert.SerializeObject(issue);
            ResponseObj response = Backend.PostBackendService(jsonObject, "/Issue", authToken);
            return response;

        }


    }
    public class Issue
    {
        public int IssueID { get; set; }
        public string? CVE { get; set; }
        public decimal CvssBaseScore { get; set; }
        public string? Cvss { get; set; }
        public string? Description { get; set; }
        public enum Status { Open = 0, InProgress = 1, Workaround = 2, Closed = 4 }
        public DateTime ReportedTime { get; set; }
        public DateTime? ClosedTime { get; set; }
        public int ReportedEmployeeID { get; set; }
        public Employee ReportedEmployee { get; set; } = new();
        //public Employee? ReportedEmployee { get { SIMSContext context = new(); return context.Employees.Find(ReportedEmployeeID); } }
        public int? AssignedEmployeeID { get; set; }
        public Employee AssignedEmployee { get; set; } = new();
        //public Employee? AssignedEmployee { get { SIMSContext context = new(); return context.Employees.Find(AssignedEmployeeID); } }
        //public List<System>? AffectedSystems { get; } = new();
    }

    public class IssueReturnObj
    {
        public int Statuscode { get; set; } = 200;
        public string Message { get; set; }
        public List<Issue>? Issue { get; set; }
        public List<Employee>? Employees { get; set; }
        public IssueReturnObj() { }
        public IssueReturnObj(int statuscode) { this.Statuscode = statuscode; }
    }
}

