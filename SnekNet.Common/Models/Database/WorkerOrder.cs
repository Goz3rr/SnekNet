using NPoco;

namespace SnekNet.Common.Models.Database
{
    [TableName("reddit.workerorders"), PrimaryKey("id")]
    public class WorkerOrder
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("circle_id")]
        public string CircleID { get; set; }

        [Column("circle_key")]
        public string CircleKey { get; set; }

        [Column("started_by")]
        public string StartedBy { get; set; }
    }
}
