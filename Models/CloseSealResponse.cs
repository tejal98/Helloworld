namespace KabaLockIntegration.Models
{
    public class CloseSealResponse
    {
        public string ConflockClosed { get; set; }  
        public string InfoStatus { get; set; }    
        public string InfoDuress { get; set; }   
        public string InfoBattery { get; set; }
        public string Status { get; internal set; }
    }
}
