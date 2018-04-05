using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(6, "Add Admins")]
    public class _006_AddAdmins : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE reddit.admins (
                    username text,
                    added_by text,
                    PRIMARY KEY (username),
                    FOREIGN KEY (username)
                        REFERENCES reddit.tokens (username) MATCH SIMPLE
                        ON UPDATE NO ACTION
                        ON DELETE NO ACTION,
                    FOREIGN KEY (added_by)
                        REFERENCES reddit.tokens (username) MATCH SIMPLE
                        ON UPDATE NO ACTION
                        ON DELETE NO ACTION
                )
            ");
        }

        protected override void Down()
        {
            throw new System.NotImplementedException();
        }
    }
}
