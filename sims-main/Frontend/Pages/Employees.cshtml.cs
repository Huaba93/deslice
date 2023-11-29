using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS_Frontend.Classes;

namespace SIMS_Frontend.Pages
{
	public class EmployeesModel : PageModel
    {
        public List<Employee> employees { get; set; }
        public IActionResult OnGet()
        {
            string authToken = Request.Cookies["AuthToken"];
            EmployeeReturnObj retObj = EmployeeService.GetEmployees(authToken);
            if (retObj.Statuscode == 401) { return  RedirectToPage("Index"); }
            //TODO Show Fancy Error Message if not 200
            if (retObj.Statuscode != 200) { return RedirectToPage("Index"); }
            employees = retObj.Employees;
                

           return Page();
        }
    }


}
