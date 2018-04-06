using NPoco;

namespace SnekNet.Common.Models.Database
{
    [TableName("admins"), PrimaryKey("username", AutoIncrement = false)]
    public class AdminUser
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("added_by")]
        public string AddedBy { get; set; }
    }
}
