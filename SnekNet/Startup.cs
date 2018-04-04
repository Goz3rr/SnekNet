using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NPoco;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;
using SnekNet.Api;
using SnekNet.Api.Reddit;

namespace SnekNet
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var provider = new PostgresqlDatabaseProvider(new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            var migrator = new SimpleMigrator(typeof(Database.Migrations._001_Tokens).GetTypeInfo().Assembly, provider);

            migrator.Load();
            migrator.MigrateToLatest();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddSingleton<IApiClient, HttpApiClient>();
            services.AddSingleton<IRedditApi, RedditApi>();

            services.AddSingleton(new DatabaseFactory(new DatabaseFactoryConfigOptions
            {
                Database = () => new NPoco.Database(Configuration.GetConnectionString("DefaultConnection"), DatabaseType.PostgreSQL, NpgsqlFactory.Instance)
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
