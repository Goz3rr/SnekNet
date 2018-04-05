using System;
using SimpleMigrations;

namespace SnekNet.Database.Migrations
{
    [Migration(7, "Add worker requeue api")]
    public class _007_WorkerRequeue : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE reddit.workerorders (
                    id serial,
                    circle_id text NOT NULL,
                    circle_key text NOT NULL,
                    started_by text,
                    PRIMARY KEY (id),
                    FOREIGN KEY (started_by)
                        REFERENCES reddit.tokens (username) MATCH SIMPLE
                        ON UPDATE NO ACTION
                        ON DELETE NO ACTION
                );

                CREATE TABLE reddit.workertasks (
                    id serial,
                    order_id integer NOT NULL,
                    username text NOT NULL,
                    executed_by text,
                    executed_at timestamp with time zone DEFAULT NULL,
                    PRIMARY KEY (id),
                    FOREIGN KEY (order_id)
                        REFERENCES reddit.workerorders (id) MATCH SIMPLE
                        ON UPDATE NO ACTION
                        ON DELETE CASCADE,
                    FOREIGN KEY (username)
                        REFERENCES reddit.tokens (username) MATCH SIMPLE
                        ON UPDATE NO ACTION
                        ON DELETE CASCADE
                );
            ");

            //Execute("DROP TABLE reddit.workerqueue");
        }

        protected override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
