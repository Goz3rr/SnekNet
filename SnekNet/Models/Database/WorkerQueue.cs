using System;
using NPoco;

namespace SnekNet.Models.Database
{
    [TableName("reddit.workerqueue")]
    public class WorkerQueue
    {
        [Column("task_id")]
        public int TaskId { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("target_id")]
        public string TargetId { get; set; }

        [Column("target_key")]
        public string TargetKey { get; set; }

        [Column("executed_by")]
        public string ExecutedBy { get; set; }

        [Column("executed_at")]
        public DateTime ExecutedAt { get; set; }
    }
}
