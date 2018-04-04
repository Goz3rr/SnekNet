using System;
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

        public async Task<IActionResult> Join([FromQuery] string id, [FromQuery] string key, [FromQuery] string password, [FromQuery] uint limit = 0)
        {
            if (password != "BigSnekBigFun")
            {
                return Forbid();
            }

            // admin/join?password=BigSnekBigFun&id=89g2zn&key=ilikecirclesandsneks&limit=10

            //http://localhost:60404/admin/join?id=89g2zn&key=ilikecirclesandsneks&token=qbWmnR0YCPMu_g9di5g22VmxMB4
            //["{\"ilikecirclesandsneks\": true}","{\"is_betrayed\": false, \"total_count\": 3, \"direction\": 1, \"circle_num_outside\": 6, \"thing_fullname\": \"t3_89g2zn\"}"]

            using (var db = dbFactory.GetDatabase())
            {
                var sql = "SELECT * FROM reddit.tokens WHERE expiresutc > extract(epoch from now() at time zone 'utc')";
                if (limit > 0)
                {
                    sql += $" LIMIT {limit}";
                }

                var users = await db.FetchAsync<UserTokenInfo>(sql);
                var tasks = new List<Task<string[]>>();

                foreach (var user in users)
                {
                    var task = new WorkerQueue()
                    {
                        //TaskId = 0,
                        TargetId = id,
                        TargetKey = key,
                        Token = user.AccesToken
                    };

                    await db.InsertAsync(task);
                }

                await Task.WhenAll(tasks);
                return Ok(tasks.Select(t => t.Result));
            }
        }

        public async Task<IActionResult> UpdateTokens([FromQuery] string password)
        {
            if (password != "BigSnekBigFun")
            {
                return Forbid();
            }

            using (var db = dbFactory.GetDatabase())
            {
                var users = await db.FetchAsync<UserTokenInfo>("SELECT * FROM reddit.tokens WHERE expiresutc < (extract(epoch from now() at time zone 'utc') - 60)");

                foreach (var user in users)
                {
                    var now = DateTimeOffset.UtcNow;
                    var response = await redditApi.RefreshAccessToken(user.RefreshToken);

                    user.AccesToken = response.access_token;
                    user.ExpiresUTC = now.ToUnixTimeSeconds() + response.expires_in;

                    await db.UpdateAsync(user);
                }
            }

            return Ok();
        }
    }
}
