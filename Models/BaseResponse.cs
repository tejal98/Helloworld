namespace KabaLockIntegration.Models
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        // public string UserToken { get; set; }
        //[DataMember(Name = "access_token")]
        // public AccessTokenResponse UserToken { get; set; }

    }
    /// <summary>
    /// Generic Base Response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T> : BaseResponse
    {
        public T Entity { get; set; }

    }
}
