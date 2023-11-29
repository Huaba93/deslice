using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS_Frontend.Classes;

namespace SIMS_Frontend.Pages
{
    public class AdminLteModel : PageModel
    {
        public List<Notification> notifications { get; set; } = new();

        public IActionResult OnGet()
        {
            string authToken = Request.Cookies["AuthToken"];
            string uid = Request.Cookies["uid"];
            if (uid is null || authToken is null)
            {
                return Page();
            }
            notifications = NotificationService.GetNotificationsForUser(int.Parse(uid), authToken);

            return Page();
        }
    }
}
