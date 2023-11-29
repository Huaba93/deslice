using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS_Frontend;
using SIMS_Frontend.Classes;
using Newtonsoft.Json;

namespace SIMS_Frontend.Pages
{
	public class DashboardModel : PageModel
    {
        public string Username { get; set; }
        
        public IActionResult OnGet()
        {
            string authToken = Request.Cookies["AuthToken"];
            string uid = Request.Cookies["uid"];
            if (authToken == string.Empty || null == authToken )
            {
                return RedirectToPage("Index");
            }
            else
            {
                TokenObj tokenobj = Authentication.ReadJwtToken(authToken);

                //TODO - Extract UID from AuthToken
                Username = Authentication.GetUserName(tokenobj.userID, authToken);
                List<Notification>notifications = NotificationService.GetNotificationsForUser(int.Parse(uid), authToken);
                Response.Cookies.Append("notification", JsonConvert.SerializeObject(notifications));
                return Page();
            }
        }


    }
}
