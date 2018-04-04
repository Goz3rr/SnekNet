using System;
using NPoco;

namespace SnekNet.Models.Database
{
    [TableName("reddit.tokens"), PrimaryKey("username", AutoIncrement = false)]
    public class UserTokenInfo
    {
        [Column("username")]
        public string Username { get; set; }

        [Column("state")]
        public Guid State { get; set; }

        [Column("accesstoken")]
        public string AccesToken { get; set; }

        [Column("tokentype")]
        public string TokenType { get; set; }

        [Column("expiresutc")]
        public long ExpiresUTC { get; set; }

        [Column("refreshtoken")]
        public string RefreshToken { get; set; }

        [Column("scope")]
        public string Scope { get; set; }
    }
}
