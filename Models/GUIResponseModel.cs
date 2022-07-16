namespace KabaLockIntegration.Models
{
    public class GUIResponseModel
    {
        public string UserState { get; set; }
        public string Status { get; set; }
        public string UserId { get;  set; }
        public string UserCode { get;  set; }
        public string UserName { get;  set; }
        public string UserDescription { get;  set; }
        public string UserInformation { get; set; }
        public string Command { get; internal set; }
        public string ErrorCode { get; set; }
        public string ErrorParameterName { get;set; }
        public string ErrorReason { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorParameterValue { get; set; } 

    }
}
