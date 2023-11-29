using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace SIMS_Frontend.Pages
{
	public class DefaultModel : PageModel
	{
        public string Username { get; set; }

        public IActionResult OnGet()
        {
            string authToken = Request.Cookies["AuthToken"];
            if (authToken == string.Empty || null == authToken)
            {
                return RedirectToPage("Index");
            }
            else
            {
                int userid = JwtService.GetUserIdFromToken(authToken); 
                Username = Authentication.GetUserName(userid, authToken);
                return Page();
            }
        }
    }
}

