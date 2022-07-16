﻿namespace KabaLockIntegration.Models
{
    public class GOCResponseModel
    {
        public string Status { get; set; }
        public string Command { get; set; }
        public string ErrorCode { get; internal set; }
        public string ErrorParameterName { get; internal set; }
        public string ErrorParameterValue { get; internal set; }
        public string ErrorReason { get; set; }
        public string ErrorMessage { get; set; }
        public string OpeningCode { get; internal set; }
    }
}
