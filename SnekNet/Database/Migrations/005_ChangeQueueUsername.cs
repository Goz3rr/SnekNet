using System;
using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(5, "Change queue to use username reference instead of tokens")]
    public class _005_ChangeQueueUsername : Migration
    {
        protected override void Up()
        {
            Execute(@"
                ALTER TABLE reddit.workerqueue DROP COLUMN token;

                ALTER TABLE reddit.workerqueue
                    ADD COLUMN username text;
                ALTER TABLE reddit.workerqueue
                    ADD FOREIGN KEY (username)
                    REFERENCES reddit.tokens (username) MATCH SIMPLE
                    ON UPDATE NO ACTION
                    ON DELETE CASCADE;
            ");
        }

        protected override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
