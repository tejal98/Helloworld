namespace KabaLockIntegration.Models
{
    public class GLIResponseModel
    {
        public string Status { get; set; }
        public string Command { get; set; }
        public string Lockcode { get; set; }
        public string Lockdescription1 { get; set; }
        public string Lockdescription2 { get; set; }
        public string Locklocation { get; set; }
        public string Lockmode { get; set; }
        public string Lockstate { get; set; }
        public string Lockgrpstate { get; set; }
        public string Lockgrpname { get; set; }
        public string ErrorCode { get; internal set; }
        public string ErrorParameterName { get; set; }
        public string ErrorReason { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorParameterValue { get; set; }
    }
}
