namespace KabaLockIntegration.Models
{
    public static class Messages
    {
        public const string Error_Message_GUI = "Error occured while getting User information from Kaba Server. ";

        public const string Error_Message_CUA = "Error occured while getting User lock assignment information from Kaba Server. ";

        public const string Error_Message_GLI = "Error occured while getting lock information from Kaba Server. ";

        public const string Error_Message_GOC = "Error occured while getting opening combination information from Kaba Server. ";

        public const string Error_Message = "Error occured while getting information from Kaba Server. ";

        public const string Err00_Message = "Unknown Error. ";

        public const string Err01_Message = "Machine Error. ";

        public const string Err02_Message = "One or more parameter is missing in request.";

        public const string Err03_Message = "One or more parameter is wrong in request.";

        public const string Err04_Message = "Impossible command passed in request. ";

        public static string UserNotAssignedToLock = "User is not assigned to the lock. ";

        public static string Error_NotInterface = "vocserveropid is not an interface operator. ";

        public static string Error_DisableInterface = "vocserveropid is disabled interface operator. ";
        
        public static string UserNotAssignedToOperator = "User is not assigned to the Operator. ";

        public static string UnknownMachine = "Unknown Machine. ";

        public static string Error_LockNotInRightState = "Lock not in the right state. ";

        public static string Error_RandomIdIsNotActivitated = "Random id is not activitated in AS274. ";

        public static string ErrorUser2IsMissing = "User 2 (InPar7, InPar8) is missing. ";

        public static string Error46_Message = "OTC has been already generated for this ATM. ";

        public static string LockAlreadyOpen = "Lock already open.Please enter close seal for this ATM and then try again to generate OTC.";

        public static string LockAlreadyClosed = "Lock not in the right state";

        public static string Error_Message_DCS = "Error occured while getting decode close seal information from Kaba Server. ";

        public static string Error34_Message = "Wrong close seal or lock not synchronized. Please first enter last close seal again and try to close.";

        public static string UserIsDisableorDelete = "User is either deleted or disabled. ";

        public static string LockGroupStateDisable = "Lock group state is disabled. ";
    }
}
