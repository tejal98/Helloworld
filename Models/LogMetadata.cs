using System.Net;

namespace KabaLockIntegration.Models
{
    public class LogMetadata
    {
        public string RequestContentType { get; set; }
        public string RequestContent { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public DateTime? RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public string ResponseContent { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
    }
}
