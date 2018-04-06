using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Npgsql;
using NPoco;
using SnekNet.Common.Models.Database;
using SnekNet.Common.Models.JSON;

namespace SnekNet.Worker
{
    internal static class Program
    {
        private const string WorkerFunctionURL = "https://us-central1-sneknet-200308.cloudfunctions.net";

        private static async Task Main(string[] args)
        {
            var dbFactory = new DatabaseFactory(new DatabaseFactoryConfigOptions
            {
                Database = () => new Database("Server=127.0.0.1;Port=5432;Database=redditcircledb;User Id=redditcircle;Password=robinwasbetter;", DatabaseType.PostgreSQL, NpgsqlFactory.Instance)
            });

            var rnd = new Random();

            var ignoredWorkers = new[] { 41, 60, 62, 93 };

            int currentWorker = 0;
            const int maxWorkers = 100;

            if (args.Length > 0)
            {
                currentWorker = int.Parse(args[0]);
            }

            using (var db = dbFactory.GetDatabase())
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(8);
                    client.DefaultRequestHeaders.Add("User-Agent", "SnekNet Backend v0.1");

                    while (true)
                    {
                        var order = await db.FirstOrDefaultAsync<WorkerTask>("SELECT ordr.circle_id, ordr.circle_key, token.accesstoken, task.* FROM reddit.workertasks as task, reddit.workerorders as ordr, reddit.tokens as token WHERE task.executed_by IS NULL AND task.order_id = ordr.id AND task.username = token.username ORDER BY task.id ASC LIMIT 1").ConfigureAwait(false);

                        if (order == null)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                            continue;
                        }

                        try
                        {
                            if (currentWorker > maxWorkers) currentWorker = 0;
                            while (ignoredWorkers.Contains(currentWorker))
                            {
                                Console.WriteLine($"Working {currentWorker} ignored");
                                currentWorker++;
                            }

                            Console.WriteLine($"Worker {currentWorker} executing order");
                            using (var response = await client.GetAsync($"{WorkerFunctionURL}/join-{currentWorker}?accessToken={order.TokenResult}&id=t3_{order.CircleId}&key={HttpUtility.UrlEncode(order.CircleKey)}").ConfigureAwait(false))
                            {
                                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                                try
                                {
                                    var json = JsonConvert.DeserializeObject<CircleVoteResponse>(body);
                                    if (json.is_betrayed == true)
                                    {

                                    }
                                    else if (json.error == 403)
                                    {
                                        await db.InsertAsync(new WorkerTask()
                                        {
                                            OrderId = order.OrderId,
                                            Username = order.Username
                                        }).ConfigureAwait(false);
                                        Console.WriteLine($"was denied, requeued");
                                    }
                                }
                                catch (JsonSerializationException)
                                { }
                                catch (JsonReaderException)
                                { }

                                Console.WriteLine(body);
                            }

                            order.ExecutedAt = DateTime.UtcNow;
                            order.ExecutedBy = $"worker-{currentWorker}";
                            await db.UpdateAsync(order).ConfigureAwait(false);

                            currentWorker++;
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine("task cancelled");
                            currentWorker++;
                        }


                        await Task.Delay(TimeSpan.FromSeconds(0.5 + rnd.NextDouble())).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
