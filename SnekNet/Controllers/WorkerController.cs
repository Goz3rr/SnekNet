using System;
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
                return StatusCode(403);
            }

            if (amount < 1) amount = 1;
            if (amount > 16) amount = 16;

            using (var db = dbFactory.GetDatabase())
            {
                var results = await db.FetchAsync<WorkerTask>($"SELECT ordr.circle_id, ordr.circle_key, token.accesstoken, task.* FROM reddit.workertasks as task, reddit.workerorders as ordr, reddit.tokens as token WHERE task.executed_by IS NULL AND task.order_id = ordr.id AND task.username = token.username ORDER BY task.id ASC LIMIT {amount}");

                var response = results.Select(x => new WorkerTaskResponse()
                {
                    id = x.Id,
                    circle_id = x.CircleId,
                    circle_key = x.CircleKey,
                    token = x.TokenResult
                });

                foreach (var result in results)
                {
                    result.ExecutedAt = DateTime.UtcNow;
                    result.ExecutedBy = HttpContext.Connection.RemoteIpAddress.ToString();
                    await db.UpdateAsync(result);
                }

                return Ok(response);
            }
        }

        [HttpGet("Requeue")]
        public async Task<IActionResult> Requeue([FromQuery] string secret, [FromQuery] int? id)
        {
            if (secret != "SssuperSssecretThing")
            {
                return StatusCode(403);
            }

            if (id == null)
            {
                return BadRequest();
            }

            using (var db = dbFactory.GetDatabase())
            {
                var task = await db.FirstOrDefaultAsync<WorkerTask>($"SELECT * FROM reddit.workertasks WHERE id = {id} AND executed_by = @0", HttpContext.Connection.RemoteIpAddress.ToString());
                if (task == null)
                {
                    return BadRequest();
                }

                await db.InsertAsync(new WorkerTask()
                {
                    OrderId = task.OrderId,
                    Username = task.Username
                });

                return Ok();
            }
        }
    }
}
