using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS_Frontend;


namespace SIMS_Frontend.Pages
{

    public class IndexModel : PageModel
    {
        const int expireTime = 13;
        public bool FailedLogin { get; set; }
        public IActionResult OnGet()
        {
            if (Request.Cookies["AuthToken"] == string.Empty || null == Request.Cookies["AuthToken"])
            {
                return Page();

            }
            else
            {
                string authToken = Request.Cookies["AuthToken"];
                TokenObj tokenObj = Authentication.ReadJwtToken(authToken);
                if (tokenObj.isValid)
                {
                    return RedirectToPage("dashboard");
                }
                else
                {
                    Response.Cookies.Delete("AuthToken");
                    return Page();
                }
            }
        }
        public IActionResult OnPost()
        {
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            ResponseObj resp = Authentication.GetAuthToken(Username, Password);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized) {

                this.FailedLogin = true;
                return Page();
            }
            else
            {
                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddMinutes(expireTime);
                Response.Cookies.Append("authToken", resp.Message ?? "",cookieOptions);
                int uid = JwtService.GetUserIdFromToken(resp.Message);
                Response.Cookies.Append("uid", uid.ToString());
                string username = Authentication.GetUserName(uid, resp.Message);
                Response.Cookies.Append("username", username);
                return RedirectToPage("dashboard");
            }
                      
        }
    }
    public class UserModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        
    }
    
}
