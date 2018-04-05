using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPoco;
using SnekNet.Api.Reddit;
using SnekNet.Models.Database;

namespace SnekNet.Controllers
{
    public class AuthController : Controller
    {
        public const string RedditState = "reddit-state";

        public IConfiguration Configuration { get; }

        private readonly IRedditApi redditApi;
        private readonly DatabaseFactory dbFactory;

        public AuthController(IConfiguration configuration, IRedditApi redditApi, DatabaseFactory dbFactory)
        {
            Configuration = configuration;
            this.redditApi = redditApi;
            this.dbFactory = dbFactory;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var redditConfig = Configuration.GetSection("Reddit");
            var state = Guid.NewGuid();

            HttpContext.Session.Set(RedditState, state.ToByteArray());
            return Redirect($"{RedditApi.BaseUrl}/authorize?client_id={redditConfig["ClientID"]}&response_type=code&state={state}&redirect_uri={redditConfig["RedirectURI"]}&duration=permanent&scope=identity,vote");
        }

        public async Task<IActionResult> Check([FromQuery] string state, [FromQuery] string code, [FromQuery] string error)
        {
            if (error != null)
            {
                return BadRequest(error);
            }

            if (!HttpContext.Session.TryGetValue(RedditState, out byte[] bytes))
            {
                return BadRequest();
            }

            var savedState = new Guid(bytes);
            if (!Guid.TryParse(state, out Guid returnedState) || savedState != returnedState)
            {
                return BadRequest();
            }

            var now = DateTimeOffset.UtcNow;
            var data = await redditApi.GetAccesToken(code);
            var user = await redditApi.GetUserData(data.access_token);

            // april 1st 2018
            if (user.created_utc > 1522540800)
            {
                return Ok("your account is too new to participate");
            }

            using (var db = dbFactory.GetDatabase())
            {
                var model = new UserTokenInfo()
                {
                    Username = user.name,
                    State = savedState,
                    AccesToken = data.access_token,
                    TokenType = data.token_type,
                    ExpiresUTC = now.ToUnixTimeSeconds() + data.expires_in.Value,
                    RefreshToken = data.refresh_token,
                    Scope = data.scope
                };

                db.Save(model);
            }

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
