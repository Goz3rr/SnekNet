using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(1, "Create tables for reddit token storage")]
    public class _001_Tokens : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE reddit.tokens (
                    username TEXT NOT NULL,
                    accesstoken TEXT,
                    tokentype TEXT,
                    expiresutc INTEGER,
                    refreshtoken TEXT,
                    scope TEXT
                );
            ");
        }

        protected override void Down()
        {
            Execute("DROP TABLE reddit.tokens;");
        }
    }
}
