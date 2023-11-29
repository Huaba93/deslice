using Microsoft.AspNetCore.Mvc;

namespace SIMS_Backend.Controllers;

public class EmployeeController : SIMSBaseControlerBase
{
    [HttpGet(Name = "GetEmployee")]
    public IActionResult Get([FromHeader] string authToken)
    {
        AuthObj auth = Auth.VerifyJWT(authToken);
        if (auth.tokenValid && Auth.IsAdmin(auth))
        {
            List<Employee> employees;
            using (SIMSContext context = ContextFactory.Create())
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
        AuthObj auth = Auth.VerifyJWT(authToken);
        if (auth.tokenValid && Auth.IsAdmin(auth))
        {
            Employee? emp;
            using (SIMSContext context = ContextFactory.Create())
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
            using (SIMSContext context = ContextFactory.Create())
            {
                context.Employees.Add(employee);
                context.SaveChanges();
                Logservice.writeLog(1, "EmployeeController", employee.UID,
                    "Employee " + employee.FirstName + " " + employee.LastName + " created", context);
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
        SIMSContext context = ContextFactory.Create();

        if (null == context.Employees.Find(id))
        {
            return NotFound("Employee not found");
        }


        try
        {
            context.ChangeTracker.Clear();
            employee.EmployeeID = id;
            context.Update(employee);
            context.SaveChanges();
            Logservice.writeLog(1, "EmployeeController", employee.UID,
                "Employee " + employee.FirstName + " " + employee.LastName + " updated", context);
            return Ok(employee);
        }
        catch (Exception ex)
        {
            return BadRequest("Unexpected Error");
        }
    }

    [HttpDelete("{id}", Name = "DeleteEmployeeById")]
    public IActionResult DeleteEmployee(int id)
    {
        SIMSContext context = ContextFactory.Create();
        Employee? emp = context.Employees.Find(id);
        if (null != emp)
        {
            context.Employees.Remove(emp);
            context.SaveChanges();
            Logservice.writeLog(1, "EmployeeController", emp.UID,
                "Employee " + emp.FirstName + " " + emp.LastName + " deleted", context);

            return Ok("Deleted");
        }
        else
        {
            return NotFound();
        }
    }

    public EmployeeController(IAuthentication authentication, ISIMSContextFactory simsContextFactory) : base(
        authentication, simsContextFactory)
    {
    }
}