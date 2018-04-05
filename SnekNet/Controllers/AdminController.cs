using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPoco;
using SnekNet.Api.Reddit;
using SnekNet.Models;
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

        private bool IsAdmin(out string name)
        {
            name = null;

            if (!HttpContext.Session.TryGetValue(AuthController.RedditState, out byte[] bytes))
            {
                return false;
            }

            var state = new Guid(bytes);

            using (var db = dbFactory.GetDatabase())
            {
                name = db.First<string>("SELECT reddit.admins.username FROM reddit.admins, reddit.tokens WHERE reddit.admins.username = reddit.tokens.username AND reddit.tokens.state = @0 ", state);
                return name != null;
            }
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin(out _))
            {
                return StatusCode(403);
            }

            using (var db = dbFactory.GetDatabase())
            {
                var viewModel = new AdminViewModel
                {
                    Admins = await db.FetchAsync<string>("SELECT username FROM reddit.admins"),
                    Users = (await db.FetchAsync<UserTokenInfo>("SELECT * FROM reddit.tokens")).Select(t => new UserData()
                    {
                        Username = t.Username,
                        Active = t.AccesToken != null,
                        TokenExpiryUTC = t.ExpiresUTC
                    }).ToList()
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin([FromForm] string username)
        {
            if (!IsAdmin(out string adminName))
            {
                return StatusCode(403);
            }

            using (var db = dbFactory.GetDatabase())
            {
                if (await db.FirstAsync<int>("SELECT COUNT(*) FROM reddit.tokens WHERE username = @0 ", username) == 0)
                {
                    return Ok("user does not exist");
                }

                await db.InsertAsync(new AdminUser()
                {
                    Username = username,
                    AddedBy = adminName,
                });
            }

            return Ok("added");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromForm] string circle_id, [FromForm] string circle_key, [FromForm] uint limit = 0)
        {
            if (!IsAdmin(out string adminName))
            {
                return StatusCode(403);
            }

            using (var db = dbFactory.GetDatabase())
            {
                var order = new WorkerOrder()
                {
                    CircleID = circle_id,
                    CircleKey = circle_key,
                    StartedBy = adminName
                };

                await db.InsertAsync(order);

                var sql = "SELECT * FROM reddit.tokens WHERE expiresutc > extract(epoch from now() at time zone 'utc') AND accesstoken IS NOT NULL ORDER BY RANDOM()";
                if (limit > 0)
                {
                    sql += $" LIMIT {limit}";
                }

                var users = await db.FetchAsync<UserTokenInfo>(sql);
                foreach (var user in users)
                {
                    await db.InsertAsync(new WorkerTask()
                    {
                        OrderId = order.Id,
                        Username = user.Username
                    });
                }

                return Ok($"queued up {users.Count} tasks");
            }
        }

        public async Task<IActionResult> UpdateTokens()
        {
            if (!IsAdmin(out string adminName))
            {
                return StatusCode(403);
            }

            int success = 0;
            var failed = new List<AccessTokenResponse>();

            using (var db = dbFactory.GetDatabase())
            {
                var users = await db.FetchAsync<UserTokenInfo>("SELECT * FROM reddit.tokens WHERE expiresutc < (extract(epoch from now() at time zone 'utc') - 60) AND accesstoken IS NOT NULL");

                await users.ParallelForEachAsync(async user =>
                {
                    var now = DateTimeOffset.UtcNow;
                    var response = await redditApi.RefreshAccessToken(user.RefreshToken);

                    user.AccesToken = response.access_token;

                    if (response.access_token != null)
                    {
                        success++;
                        user.ExpiresUTC = now.ToUnixTimeSeconds() + response.expires_in.Value;
                    }
                    else
                    {
                        failed.Add(response);
                    }

                    using (var dbParallel = dbFactory.GetDatabase())
                    {
                        await dbParallel.UpdateAsync(user);
                    }
                }, maxDegreeOfParalellism: 8);
            }

            return Ok(new
            {
                Success = success,
                Failed = failed
            });
        }
    }
}
