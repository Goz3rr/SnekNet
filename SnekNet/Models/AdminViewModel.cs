using System.Collections.Generic;

namespace SnekNet.Models
{
    public class UserData
    {
        public string Username { get; set; }
        public bool Active { get; set; }
        public long TokenExpiryUTC { get; set; }
    }

    public class AdminViewModel
    {
        public List<UserData> Users { get; set; }
    }
}