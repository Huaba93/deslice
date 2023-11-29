using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SIMS_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        [HttpGet(Name = "GetLastxLogs")]
        public List<Logs> GetLastLogs([FromQuery] int? count)
        {
            List<Logs> logs;

            if (null == count)
            {
                using (SIMSContext context = new())
                {
                    logs = context.Logs.ToList();
                }

            }
            else
            {
                using (SIMSContext context = new())
                {
                    logs = context.Logs.OrderByDescending(l => l.Created).Take(count.Value).ToList();
                    logs = logs.OrderBy(l => l.Created).ToList();
                }
            }
            return logs;

            }
    }

}

