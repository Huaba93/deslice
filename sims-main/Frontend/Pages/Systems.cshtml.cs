using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS_Frontend.Classes;

namespace SIMS_Frontend.Pages
{
	public class SystemModel : PageModel
    {
        public List<SIMS_Frontend.Classes.System> systems { get; set; }
        public List<Systemtype> systemtypes { get; set; }
        public List<Manufactor> manufactors { get; set; }
        public IActionResult OnGet()
        {
            string authToken = Request.Cookies["AuthToken"];
            SystemReturnObj retObj = SystemService.GetSystems(authToken);
            if (retObj.Statuscode == 401) { return  RedirectToPage("Index"); }
            //TODO Show Fancy Error Message if not 200
            if (retObj.Statuscode != 200) { return RedirectToPage("Index"); }
            systems = retObj.Systems;
            systemtypes = retObj.Systemtypes;
            manufactors = retObj.Manufactors;
            

           return Page();
        }
    }


}
