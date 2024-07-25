namespace ReverseProxy
{
    public class MyRateLimitOptions
    {
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int SegmentsPerWindow { get; set; }
        public int QueueLimit { get; set; }
    }
}
