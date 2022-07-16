namespace KabaLockIntegration.Models
{
    public class CDSResponseModel
    {
        public string Command { get; internal set; }
        public string Status { get; internal set; }
        public string ConfLockClosed { get; internal set; }
        public string InfoStatus { get; internal set; }
        public string InfoDuress { get; internal set; }
        public string InfoBattery { get; internal set; }
        public string ErrorCode { get; internal set; }
        public string ErrorParameterName { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public string ErrorParameterValue { get; internal set; }
        public string ErrorReason { get; internal set; }
    }
}
