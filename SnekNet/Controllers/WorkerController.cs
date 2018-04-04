using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SnekNet.Models;

namespace SnekNet.Controllers
{
    [Route("api/[controller]")]
    public class WorkerController : Controller
    {
        [HttpGet("Task")]
        public IActionResult Task([FromQuery] string secret, [FromQuery] int amount = 1)
        {
            if (secret != "SssuperSssecretThing")
            {
                return Forbid();
            }

            if (amount < 1) amount = 1;
            if (amount > 100) amount = 100;

            var tasks = new List<WorkerTask>();
            


            return Ok(tasks);
        }
    }
}
