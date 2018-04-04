using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPoco;
using SnekNet.Api.Reddit;
using SnekNet.Models.Database;

namespace SnekNet.Controllers
{
    public class AdminController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly IRedditApi redditApi;
        private readonly DatabaseFactory dbFactory;

        public AdminController(IConfiguration configuration, IRedditApi redditApi, DatabaseFactory dbFactory)
        {
            Configuration = configuration;
            this.redditApi = redditApi;
            this.dbFactory = dbFactory;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Join([FromQuery] string id, [FromQuery] string key, [FromQuery] string password)
        {
            if(password != "BigSnekBigFun")
            {
                return Forbid();
            }

            //http://localhost:60404/admin/join?id=89g2zn&key=ilikecirclesandsneks&token=qbWmnR0YCPMu_g9di5g22VmxMB4
            //["{\"ilikecirclesandsneks\": true}","{\"is_betrayed\": false, \"total_count\": 3, \"direction\": 1, \"circle_num_outside\": 6, \"thing_fullname\": \"t3_89g2zn\"}"]

            using (var db = dbFactory.GetDatabase())
            {
                // Search the DB for tokens
                var users = await db.FetchAsync<UserTokenInfo>("SELECT * FROM reddit.tokens WHERE expiresutc > extract(epoch from now() at time zone 'utc');");
                var tasks = new List<Task<string[]>>();

                foreach (var user in users)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        return new string[] {
                            await redditApi.UnlockCircle(user.AccesToken, id, key),
                            await redditApi.JoinCircle(user.AccesToken, id)
                        };
                    }));
                }

                await Task.WhenAll(tasks);
                return Ok(tasks.Select(t => t.Result));
            }
        }
    }
}
