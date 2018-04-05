using System.Collections.Generic;

namespace SnekNet.Models
{
    public class UserData
    {
        public string Username { get; set; }
        public bool Active { get; set; }
        public long TokenExpiryUTC { get; set; }
    }

    public class Order
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public int TotalCount { get; set; }
        public int ExecutedCount { get; set; }
    }

    public class AdminViewModel
    {
        public List<string> Admins { get; set; }

        public List<UserData> Users { get; set; }

        public List<Order> Orders { get; set; }
    }
}