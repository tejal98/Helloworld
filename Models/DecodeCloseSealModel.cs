namespace KabaLockIntegration.Models
{
    public class DecodeCloseSealModel
    {
        public string OperatorId { get; set; }
        public string UserCodeOrName { get; set; }
        public string LockNameOrSerialNumber { get; set; }
        public string CloseSeal { get; set; }
    }
}
