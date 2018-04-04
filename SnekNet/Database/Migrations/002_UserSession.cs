using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(2, "Save user state so they don't have to log in every time")]
    public class _002_UserSession : Migration
    {
        protected override void Up()
        {
            Execute(@"
                ALTER TABLE reddit.tokens
                ADD COLUMN state UUID;
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE reddit.tokens
                DROP COLUMN state;
            ");
        }
    }
}
