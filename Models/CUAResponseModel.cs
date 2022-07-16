namespace KabaLockIntegration.Models
{
    public class CUAResponseModel
    {
        public string Status { get;set; }
        public string Command { get;set; }    
        public string Assign { get;set; }
        public string ErrorCode { get; internal set; }
        public string ErrorParameterName { get; internal set; }
        public string ErrorParameterValue { get; internal set; }
        public string ErrorReason { get; set; }
        public string ErrorMessage { get; set; }
    }
}
