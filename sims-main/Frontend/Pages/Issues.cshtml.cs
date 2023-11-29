using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS_Frontend.Classes;
using Microsoft.Extensions.Caching.Distributed;
using SIMS_Frontend.Extensions;
using System.Security.Cryptography;

namespace SIMS_Frontend.Pages
{
	public class IssueModel : PageModel
    {
        public List<Issue> issues { get; set; }
        public Issue newIssue { get; set; }
        public List<Employee> employees { get; set; }
        public string cveLink = "https://cve.mitre.org/cgi-bin/cvename.cgi?name=";
        public IActionResult OnGet()
        {
            string authToken = Request.Cookies["AuthToken"];
            IssueReturnObj retObj = IssueService.GetIssues(authToken);
            if (retObj.Statuscode == 401) { return  RedirectToPage("Index"); }
            //TODO Show Fancy Error Message if not 200
            if (retObj.Statuscode != 200) { return RedirectToPage("Index"); }
            issues = retObj.Issue;
            employees = retObj.Employees;
            newIssue = LoadPreviousForm(Request.Cookies["uid"]).Result;

           return Page();
        }
        public IActionResult OnPost()
        {

            string? authToken = Request.Cookies["AuthToken"];
            if (authToken == null) { return BadRequest("No AuthToken"); }

            EmployeeReturnObj employeeReturn = EmployeeService.GetEmployees(authToken);
            if (employeeReturn.Employees == null || employeeReturn.Employees.Count == 0) { return NotFound("Employees not found - Backend Problem"); }
            employees = employeeReturn.Employees;

                        //TODO Fancy Input Validation
                        Issue issue = new();
                        decimal cvssbase;
                if (Request.Form.Count == 0) { return BadRequest("No Form Values delivered"); }
                        issue.CVE = Request.Form["cve"];
                        decimal.TryParse(Request.Form["cvssbase"], out cvssbase);
                        issue.CvssBaseScore = cvssbase;
                        issue.Cvss = Request.Form["cvss"];
                        issue.Description = Request.Form["description"];
                        if (Request.Form["reportedEmployee"] != "")
                        {
                            issue.ReportedEmployeeID = employees.FirstOrDefault(e => e.Name == Request.Form["reportedEmployee"]).EmployeeID;
                        }
                        if (Request.Form["assignEmployee"] != "")
                        {
                            issue.AssignedEmployeeID = employees.FirstOrDefault(e => e.Name == Request.Form["assignEmployee"]).EmployeeID;
                        }

                        issue.ReportedTime = DateTime.Now;
                        IssueService.PostIssue(authToken, issue);
            string uid = Request.Cookies["uid"] ?? "";
            if (uid != "") { _ = ClearCache(uid); }
            
            return RedirectToPage("Issues");
        }

        public async void OnPostRedis([FromForm] RedisRespObj respObj)
        {
            Console.WriteLine(respObj.ToString());
            string? authToken = Request.Cookies["AuthToken"];
            if (authToken == null) { return; }

            EmployeeReturnObj employeeReturn = EmployeeService.GetEmployees(authToken);
            if (employeeReturn.Employees == null || employeeReturn.Employees.Count == 0) { return; }
            employees = employeeReturn.Employees;

            string uid = Request.Cookies["uid"];
            await _cache.SetRecordAsync<string>(uid + "_newIssue_" + respObj.name, respObj.value, new TimeSpan(1, 0, 0)); 



        }
        public IssueModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        private IDistributedCache _cache;
        async Task SaveRedis(IFormCollection form, string uid, List<Employee> employees)
        {

            Employee repEmployee = employees.Where(e => e.Name == form["reportedEmployee"]).FirstOrDefault() ?? new();
            Employee assingEmployee = employees.Where(e => e.Name == form["assignEmployee"]).FirstOrDefault() ?? new();
            Issue issue = new()
            {
                CVE = form["cve"],
                Cvss = form["cvssbase"],
                Description = form["description"],
                ReportedEmployee = repEmployee,
                AssignedEmployee = assingEmployee
            };
            
            await _cache.SetRecordAsync<Issue>(uid + "_newIssueForm", issue, new TimeSpan(1, 0, 0));
            

        }
        async Task<Issue> LoadPreviousForm(string uid ){

            string cvssBaseString = await _cache.GetRecordAsync<string>(uid + "_newIssue_cvssbase");
            decimal.TryParse(cvssBaseString, out decimal cvssBase);

            Issue issue = new()
            {
                CVE = await _cache.GetRecordAsync<string>(uid + "_newIssue_cve"),
                Cvss = await _cache.GetRecordAsync<string>(uid + "_newIssue_cvss") ?? "",
                CvssBaseScore = cvssBase,
                Description = await _cache.GetRecordAsync<string>(uid + "_newIssue_description") ?? ""
            };
            return issue;

        }
        async Task ClearCache(string uid)
        {
            string[] keysToClear = { "cve", "cvss", "cvssbase", "description" };
            foreach (string key in keysToClear)
            {
                _cache.RemoveAsync(uid + "_newIssue_" + key);
            }
        }
        public class RedisRespObj
        {
            public string name { get; set; }
            public string value { get; set; }
        }



    }



}
