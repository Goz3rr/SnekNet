using System;
using System.Collections.Generic;
using System.Text;

namespace SnekNet.Common.Models.JSON
{
    public class CircleVoteResponse
    {
        public bool? is_betrayed { get; set; }
        public int? total_count { get; set; }
        public int? direction { get; set; }
        public int? circle_num_outside { get; set; }
        public string thing_fullname { get; set; }
        public string message { get; set; }
        public int? error { get; set; }
    }

}
