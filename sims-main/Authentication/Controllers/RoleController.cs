using Microsoft.AspNetCore.Mvc;

namespace SIMS_Authentication.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    [HttpGet(Name = "GetRoles")]
    public List<Role> Get()
    {
        AuthContext context = new();
        return context.Roles.ToList<Role>();

    }
    [HttpPost(Name = "PostRole")]
    public IActionResult CreateRole(Role role)
    {
        AuthContext context = new();
        if (context.Roles.Count(r => r.Name == role.Name) == 1) { return BadRequest("Role already exists"); }

        
        context.Roles.Add(role);
        context.SaveChanges();

        return Ok();
    }
   
    
}

