using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace SIMS_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    
    [HttpGet(Name ="GetEmployee")]
    public IActionResult Get([FromHeader]string authToken)
    {
        AuthObj auth = Authentication.VerifyJWT(authToken);
        if (auth.tokenValid && Authentication.isAdmin(auth))
        {
            List<Employee> employees;
            using (SIMSContext context = new())
            {
                employees = context.Employees.ToList<Employee>();
            }

            return Ok(employees);
        }
        else
        {
         return Unauthorized();
        }
       

    }
    [HttpGet("{id}", Name = "GetEmployeeByID")]
    public IActionResult Get(int id, [FromHeader] string authToken)
    {
        AuthObj auth = Authentication.VerifyJWT(authToken);
        if (auth.tokenValid && Authentication.isAdmin(auth))
        {
            Employee? emp;
            using (SIMSContext context = new())
            {
                emp = context.Employees.Find(id);
            }
            if (emp != null)
            {
                return Ok(emp);
            }
            else
            {
                return NotFound();
            }
        }
        else
        {
            return Unauthorized();
        }
        
    }
    [HttpPost(Name = "PostEmployee")]
    public IActionResult CreateEmployee([FromBody] Employee employee)
    {
        try
        {
            //TODO - Check Using in allen Methoden
            using (SIMSContext context = new()) 
            {
                context.Employees.Add(employee);
                context.SaveChanges();
                Logservice.writeLog(1, "EmployeeController", employee.UID, "Employee " + employee.FirstName + " " + employee.LastName + " created");
            }
            return Created("", employee);

        }
        catch
        {
            return BadRequest();
        }
        
    }
    [HttpPut("{id}", Name = "UpdateEmployee")]
    public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
    {
        SIMSContext context = new();

        if (null == context.Employees.Find(id)) { return NotFound("Employee not found"); }


        
        try
        {
            context.ChangeTracker.Clear();
            employee.EmployeeID = id;
            context.Update(employee);
            context.SaveChanges();
            Logservice.writeLog(1, "EmployeeController", employee.UID, "Employee " + employee.FirstName + " " + employee.LastName + " updated");
            return Ok(employee);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }

    [HttpDelete("{id}", Name = "DeleteEmployeeById")]
    public IActionResult DeleteEmployee(int id)
    {
        SIMSContext context = new();
        Employee ?emp = context.Employees.Find(id);
        if (null != emp)
        {
            context.Employees.Remove(emp);
            context.SaveChanges();
            Logservice.writeLog(1, "EmployeeController", emp.UID, "Employee " + emp.FirstName + " " + emp.LastName + " deleted");

            return Ok("Deleted");
        }
        else
        {
            return NotFound();
        }


    }

}



