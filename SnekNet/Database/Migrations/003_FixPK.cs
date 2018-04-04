using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(3, "Fix PK on tokens table")]
    public class _003_FixPK : Migration
    {
        protected override void Up()
        {
            Execute("ALTER TABLE reddit.tokens ADD PRIMARY KEY (username);");
        }

        protected override void Down()
        {
            Execute("ALTER TABLE reddit.tokens DROP CONSTRAINT tokens_pkey;");
        }
    }
}
