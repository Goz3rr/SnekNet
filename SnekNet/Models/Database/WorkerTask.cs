using System;
using NPoco;

namespace SnekNet.Models.Database
{
    [TableName("reddit.workertasks"), PrimaryKey("id")]
    public class WorkerTask
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [ResultColumn("accesstoken")]
        public string TokenResult { get; set; }

        [ResultColumn("circle_id")]
        public string CircleId { get; set; }

        [ResultColumn("circle_key")]
        public string CircleKey { get; set; }

        [Column("executed_by")]
        public string ExecutedBy { get; set; }

        [Column("executed_at")]
        public DateTime? ExecutedAt { get; set; }
    }
}
