namespace KabaLockIntegration.Models
{
    public class Apilog
    {
        public int RequestId { get; set; }
        public string? RequestContentType { get; set; }
        public string? RequestContent { get; set; }
        public string? RequestUri { get; set; }
        public string? RequestMethod { get; set; }
        public DateTime? RequestTimestamp { get; set; }
        public string? ResponseContentType { get; set; }
        public string? ResponseContent { get; set; }
        public string? ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
    }
}
