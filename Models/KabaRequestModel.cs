namespace KabaLockIntegration.Models
{
    public class KabaRequestModel
    {
        public string OperatorId { get; set; }
        public string UserCodeOrName { get; set; }
        public string LockNameOrSerialNumber { get; set; }
        public string OpeningMode { get; set; }
    }
}
