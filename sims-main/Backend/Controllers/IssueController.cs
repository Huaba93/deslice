using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace SIMS_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class IssueController : ControllerBase
{
    [HttpGet("{id}",Name = "GetIssueById")]
    public IActionResult Get(int id)
    {
        SIMSContext context = new();
        Issue? issue = context.Issues.Find(id);
        context.Entry(issue).Collection(s => s.AffectedSystems).Load();
        if (null == issue) { return NotFound(); } else { return Ok(issue); }
    }
    [HttpGet(Name = "GetIssue")]
    public List<Issue> Get()
    {
        SIMSContext context = new();
        return context.Issues.ToList<Issue>();
      
    }
    [HttpPost(Name = "PostIssue")]
    public IActionResult CreateIssue([FromBody] Issue issue)
    {
        SIMSContext context = new();

        if ((issue.AssignedEmployeeID != 0 || context.Employees.Find(issue.AssignedEmployeeID) == null) && context.Employees.Find(issue.ReportedEmployeeID) == null)
            {
            return NotFound("Employee not found");
            }
        try
        {
            
            context.Add(issue);
            context.SaveChanges();
            return Created("", issue);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }
        
    }
    [HttpPut("{id}",Name = "UdpateIssue")]
    public IActionResult UpdateIssue(int id, [FromBody] Issue issue)
    {
        SIMSContext context = new();
       
        if (null == context.Issues.Find(id)) { return NotFound("Issue not found"); }

        
        if ((issue.AssignedEmployeeID != 0 || context.Employees.Find(issue.AssignedEmployeeID) == null) && context.Employees.Find(issue.ReportedEmployeeID) == null)
        {
            return NotFound("Employee not found");
        }
        try
        {
            context.ChangeTracker.Clear();
            issue.IssueID = id;
            context.Update(issue);
            context.SaveChanges();
            return Ok(issue);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }
    
    [HttpDelete("{id}", Name = "DeleteIssueById")]
    public IActionResult DeleteIssue(int id)
    {
        SIMSContext context = new();
        Issue? issue = context.Issues.Find(id);
        if (null != issue)
        {
            context.Issues.Remove(issue);
            context.SaveChanges();
            return Ok("Deleted");
        }
        else
        {
            return NotFound();
        }


    }

    [HttpPut("{id}/close", Name = "CloseIssue")]
    public IActionResult UpdateIssue(int id)
    {
        SIMSContext context = new();
        Issue? issuedb = context.Issues.Find(id);
        if (null == issuedb) { return NotFound("Issue not found"); }


        try
        {
            issuedb.ClosedTime = DateTime.Now;
            context.SaveChanges();
            return Ok(issuedb);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }

    [HttpPut("{id}/assign/{employeeid}", Name = "AssignIssue")]
    public IActionResult UpdateIssue(int id, int employeeid)
    {
        SIMSContext context = new();
        Issue? issuedb = context.Issues.Find(id);
        if (null == issuedb) { return NotFound("Issue not found"); }

        if (null == context.Employees.Find(employeeid)) { return NotFound("Employee not found"); }

        try
        {
            issuedb.AssignedEmployeeID = employeeid;
            context.SaveChanges();
            return Ok(issuedb);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }

    [HttpPut("{id}/affectedSystem/", Name = "AffectedSystem")]
    public IActionResult UpdateIssueAffectedSystems(int id,[FromQuery] int[] affectedSystems)
    {

        Issue issue = new();
        int countSuccess = 0;
        using (SIMSContext context = new())
        {
            issue = context.Issues.Include(i => i.AffectedSystems).Where(i => i.IssueID == id).First();

            List<int> issueAffectedSystems = issue.AffectedSystems.Select(i => i.SystemId).ToList();
            if (null == issue) { return NotFound("Issue not found"); }

            // if (null == issue.AffectedSystems) { issue.AffectedSystems = new List<System>(); }
            
            foreach (int sysid in affectedSystems)
            {
                using (SIMSContext syscont = new())
                {

                    System? sys = syscont.Systems.Find(sysid);
                    if (null != sys)
                    {
                        if (!issueAffectedSystems.Contains(sys.SystemId)) 
                        {
                            issue.AffectedSystems.Add(sys);
                            countSuccess++;
                        }
                        
                    }
                    syscont.SaveChanges();
                }
            }
            if(countSuccess == 0)
            {
                return Ok("No Changes");
            }
            context.Update(issue);
            context.SaveChanges();
        }

        return Ok(countSuccess.ToString() + " System(s) assigned Sucessfully");

    }

    [HttpPut("{id}/escalate/", Name = "Escalate")]
    public IActionResult UpdateIssueEscalate(int id)
    {
        SIMSContext context = new();
        Issue? issue = context.Issues.Find(id);
        if (null == issue) { return NotFound("Issue not found"); }
        Employee? emp = context.Employees.Find(issue.AssignedEmployeeID);
        if (null == emp) { return NotFound("Assigned Employee not found"); }
        if (null == emp.SuperiorID) { return NotFound("No Superior Employee defined!"); }
        issue.AssignedEmployeeID = emp.SuperiorID;
        Employee Superior = context.Employees.Find(emp.SuperiorID);
        if (null == Superior) { return NotFound("Superior User Object not found"); }
        context.Update(issue);
        context.SaveChanges();
        return Ok("Employee new assigned to "+ Superior.FirstName + " " + Superior.LastName); ;

    }

}



