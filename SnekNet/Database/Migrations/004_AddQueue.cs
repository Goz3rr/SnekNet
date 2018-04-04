
using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(4, "add worker queue tables")]
    public class _004_AddQueue : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE reddit.workerqueue (
                    task_id INTEGER NOT NULL,
                    token TEXT NOT NULL,
                    target_id TEXT NOT NULL,
                    target_key TEXT NOT NULL,
                    executed_by TEXT,
                    executed_at timestamp with time zone
                );
            ");
        }

        protected override void Down()
        {
            Execute("DROP TABLE reddit.workerqueue");
        }
    }
}
