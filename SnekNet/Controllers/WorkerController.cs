using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPoco;
using SnekNet.Models;
using SnekNet.Models.Database;

namespace SnekNet.Controllers
{
    [Route("api/[controller]")]
    public class WorkerController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly DatabaseFactory dbFactory;

        public WorkerController(IConfiguration configuration, DatabaseFactory dbFactory)
        {
            Configuration = configuration;
            this.dbFactory = dbFactory;
        }

        [HttpGet("Task")]
        public async Task<IActionResult> Task([FromQuery] string secret, [FromQuery] int amount = 1)
        {
            if (secret != "SssuperSssecretThing")
            {
                return Forbid();
            }

            if (amount < 1) amount = 1;
            if (amount > 16) amount = 16;

            using (var db = dbFactory.GetDatabase())
            {
                //var tasks = new List<WorkerTask>();
                var results = await db.FetchAsync<WorkerQueue>($"SELECT * FROM reddit.workerqueue WHERE executed_by IS NULL ORDER BY task_id ASC LIMIT {amount}");

                var tasks = results.Select(x => new WorkerTask()
                {
                    id = x.TargetId,
                    key = x.TargetKey,
                    token = x.Token
                });

                foreach(var result in results)
                {
                    result.ExecutedAt = DateTime.UtcNow;
                    result.ExecutedBy = HttpContext.Connection.RemoteIpAddress.ToString();
                    await db.UpdateAsync(result);
                }

                return Ok(tasks);
            }
        }
    }
}
