using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace SIMS_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemController : ControllerBase
{

    [HttpGet(Name ="GetSystem")]
    public List<System> GetSystem()
    {
        var context = new SIMSContext();
       return context.Systems.ToList<System>();

    }
    [HttpGet("{id}", Name = "GetSystemByID")]
    public IActionResult GetSystem(int id)
    {
        SIMSContext context = new();
        System ?system = context.Systems.Find(id);
        if (system != null)
        {
            return Ok(system);
        }else
        {
            return NotFound();
        }
    }
    [HttpPost(Name = "PostSystem")]
    public IActionResult CreateSystem([FromBody] System system)
    {
        SIMSContext subcontext = new();
        Systemtype? systemtype = subcontext.Systemtypes.Find(system.SystemtypeID);
        Manufactor? manufactor = subcontext.Manufactors.Find(system.ManufactorID);

        if (null == systemtype && null == manufactor) { return NotFound("Systemtype and Manufactor not found"); }
        if (null == systemtype) { return NotFound("Systemtype not found"); }
        if (null == manufactor) { return NotFound("Manufactor not found"); }

        try
       {
            SIMSContext context = new();
            context.Systems.Add(system);
            context.SaveChanges();
            return Created("", system);
       }
       catch
       {
          return BadRequest();
       }
        
    }
    [HttpPut("{id}", Name = "UpdateSystem")]
    public IActionResult UpdateSystem(int id, [FromBody] System system)
    {
        SIMSContext context = new();

        if (null == context.Systems.Find(id)) { return NotFound("System not found"); }


        
        try
        {
            context.ChangeTracker.Clear();
            system.SystemId = id;
            context.Update(system);
            context.SaveChanges();
            return Ok(system);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }

    [HttpDelete("{id}", Name = "DeleteSystemById")]
    public IActionResult DeleteSystem(int id)
    {
        SIMSContext context = new();
        System ?emp = context.Systems.Find(id);
        if (null != emp)
        {
            context.Systems.Remove(emp);
            context.SaveChanges();
            return Ok("Deleted");
        }
        else
        {
            return NotFound();
        }


    }

}

[ApiController]
[Route("[controller]")]
public class SystemTypeController : Controller
{
    [HttpGet(Name = "GetSystemType")]
    public List<Systemtype> GetSystemType()
    {
        var context = new SIMSContext();
        return context.Systemtypes.ToList<Systemtype>();

    }
    [HttpGet("{id}", Name = "GetSystemTypeByID")]
    public IActionResult GetSystemType(int id)
    {
        SIMSContext context = new();
        Systemtype? systemType = context.Systemtypes.Find(id);
        if (systemType != null)
        {
            return Ok(systemType);
        }
        else
        {
            return NotFound();
        }
    }
    [HttpPost(Name = "PostSystemType")]
    public IActionResult CreateSystemType([FromBody] Systemtype systemtype)
    {
        try
        {
            SIMSContext context = new();
            context.Systemtypes.Add(systemtype);
            context.SaveChanges();
            return Created("", systemtype);
        }
        catch
        {
            return BadRequest();
        }

    }
    [HttpPut("{id}", Name = "UpdateSystemType")]
    public IActionResult UpdateSystemType(int id, [FromBody] Systemtype systemtype)
    {
        SIMSContext context = new();

        if (null == context.Systemtypes.Find(id)) { return NotFound("Systemtype not found"); }



        try
        {
            context.ChangeTracker.Clear();
            systemtype.SystemTypeId = id;
            context.Update(systemtype);
            context.SaveChanges();
            return Ok(systemtype);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }

    [HttpDelete("{id}", Name = "DeleteSystemTypeById")]
    public IActionResult DeleteSystemType(int id)
    {
        SIMSContext sysContext = new();
        if (null != sysContext.Systems.Where(s => s.SystemtypeID == id).FirstOrDefault()) { return BadRequest("There are Systems with this Systemtype assigned - Deletion forbidden"); };
        SIMSContext context = new();
        Systemtype? systemtype = context.Systemtypes.Find(id);
        if (null != systemtype)
        {
            context.Systemtypes.Remove(systemtype);
            context.SaveChanges();
            return Ok("Deleted");
        }
        else
        {
            return NotFound();
        }


    }

}

[ApiController]
[Route("[controller]")]
public class ManufactorController : Controller
{
    [HttpGet(Name = "GetManufactor")]
    public List<Manufactor> GetManufactor()
    {
        var context = new SIMSContext();
        return context.Manufactors.ToList<Manufactor>();

    }
    [HttpGet("{id}", Name = "GetManufactorById")]
    public IActionResult GetManufactor(int id)
    {
        SIMSContext context = new();
        Manufactor? manufactor = context.Manufactors.Find(id);
        if (manufactor != null)
        {
            return Ok(manufactor);
        }
        else
        {
            return NotFound();
        }
    }
    [HttpPost(Name = "PostManufactor")]
    public IActionResult CreateManufactor([FromBody] Manufactor manufactor)
    {
        try
        {
            SIMSContext context = new();
            context.Manufactors.Add(manufactor);
            context.SaveChanges();
            return Created("", manufactor);
        }
        catch
        {
            return BadRequest();
        }

    }
    [HttpPut("{id}", Name = "UpdateManufactor")]
    public IActionResult UpdateManufactor(int id, [FromBody] Manufactor manufactor)
    {
        SIMSContext context = new();

        if (null == context.Manufactors.Find(id)) { return NotFound("Manufactor not found"); }



        try
        {
            context.ChangeTracker.Clear();
            manufactor.ManufactorId = id;
            context.Update(manufactor);
            context.SaveChanges();
            return Ok(manufactor);
        }
        catch
        {
            return BadRequest("Unexpected Error");
        }

    }

    [HttpDelete("{id}", Name = "DeleteManufactorById")]
    public IActionResult DeleteManufactor(int id)
    {
        SIMSContext manContext = new();
        if (null != manContext.Manufactors.Where(m => m.ManufactorId == id).FirstOrDefault()) { return BadRequest("There are Systems with this Manufactor assigned - Deletion forbidden"); };

        SIMSContext context = new();
        Manufactor? manufactor = context.Manufactors.Find(id);
        if (null != manufactor)
        {
            context.Manufactors.Remove(manufactor);
            context.SaveChanges();
            return Ok("Deleted");
        }
        else
        {
            return NotFound();
        }


    }

}



