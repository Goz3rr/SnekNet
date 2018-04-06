namespace SnekNet.Api.Reddit
{ 
    public class CircleResponse
    {
        public string kind { get; set; }
        public Data data { get; set; }

        public string message { get; set; }
        public int? error { get; set; }
    }

    public class Data
    {
        public object after { get; set; }
        public object whitelist_status { get; set; }
        public string modhash { get; set; }
        public int? dist { get; set; }
        public Child[] children { get; set; }
        public object before { get; set; }
    }

    public class Child
    {
        public string kind { get; set; }
        public Data1 data { get; set; }
    }

    public class Data1
    {
        public bool is_betrayed { get; set; }
    }

}
