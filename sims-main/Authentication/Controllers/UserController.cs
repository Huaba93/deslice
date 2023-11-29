using Microsoft.AspNetCore.Mvc;

namespace SIMS_Authentication.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet(Name = "GetUser")]
    public List<User> Get()
    {
        AuthContext context = new();
        return context.Users.ToList<User>();

    }
    [HttpGet("{id}", Name = "GetUserbyId")]
    public IActionResult GetUserbyID(int id, [FromHeader] string authToken)
    {
        if (Authentication.VerifyJWT(authToken))
        {
            AuthContext context = new();
            return Ok(context.Users.Find(id));
        }
        else
            return Unauthorized();

    }
    [HttpPost(Name = "PostUser")]
    public IActionResult CreateUser(CreateUserDTO user)
    {
        AuthContext context = new();
        if (context.Users.Count(u => u.Username == user.Username) == 1) { return BadRequest("User already exists"); }

        User usr = new(); usr.Username = user.Username; usr.Firstname = user.Firstname; usr.Lastname = user.Lastname; usr.RoleID = user.RoleID;

        // Create Strong Passsword

        string hash = Authentication.PasswordHash(user.Password, out byte[] salt) ?? "";

        usr.Password = hash;
        usr.Salt = salt;
        context.Users.Add(usr);
        context.SaveChanges();


        return Ok();
    }

    [HttpDelete("{id}",Name="DeleteUser")]
    public IActionResult DeleteUser(int id)
    {
        AuthContext context = new();
        User? usr = context.Users.Find(id);
        if (null == usr)
        {
            return NotFound("User not found");
        }else
        {
            context.Users.Remove(usr);
            context.SaveChanges();
            return Ok("User deleted");    
        }
    }
    
}

