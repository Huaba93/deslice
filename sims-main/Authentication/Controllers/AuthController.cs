using System;
using Microsoft.AspNetCore.Mvc;

namespace SIMS_Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost(Name = "Auth")]
        public IActionResult AuthUser([FromBody] Auth auth)
        {
            AuthContext context = new();
            User? usr = context.Users.Where(u => u.Username == auth.Username).FirstOrDefault();
            if (null == usr)
            {
                // Prevent Timing Attack ;)
                Random waitTime = new Random();
                System.Threading.Thread.Sleep(waitTime.Next(200));
                return Unauthorized("Username or Password wrong!");
            }
            else
            {
                if (Authentication.VerifyPassword(auth.Password, usr.Password, usr.Salt))
                {
                    string jwtToken = Authentication.GenerateAuthToken(usr);
                    return Ok(jwtToken);
                }
                else
                {
                    // Prevent Timing Attack ;)
                    Random waitTime = new Random();
                    System.Threading.Thread.Sleep(waitTime.Next(200));
                    return Unauthorized("Username or Password wrong!");
                }

            }

        }

        [HttpGet("pubkey", Name = "GetPublicKey")]
        public IActionResult GetPublicKey()
        {
            if (System.Environment.GetEnvironmentVariable("ecPublicKey") != null)
            {
                return Ok(System.Environment.GetEnvironmentVariable("ecPublicKey"));
            }
            else
            {
                Authentication.GenerateECKey();
                if (System.Environment.GetEnvironmentVariable("ecPublicKey") != null)
                {
                    return Ok(System.Environment.GetEnvironmentVariable("ecPublicKey"));
                }
                else
                {
                    return BadRequest();
                }

            }

        }
    }
}

