using AutoMapper;
using KabaLockIntegration.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KabaLockIntegration.Utility
{
    public class Logging
    {
        public void DbLog(string requestMethod, string URL, dynamic requestContent, dynamic responseContent, KabaDBContext _context)
        {
            try
            {
                LogMetadata log1 = new LogMetadata();

                log1.RequestMethod = requestMethod;//"OpenLockA";
                log1.RequestTimestamp = DateTime.Now;
                log1.RequestUri = URL;//snGService.ServiceURL.ToString();
                log1.RequestContentType = "application/json";
                if (!string.IsNullOrEmpty(requestContent))
                    log1.RequestContent = JsonSerializer.Serialize(requestContent);
                
                if(!string.IsNullOrEmpty(responseContent))
                    log1.ResponseContent = JsonSerializer.Serialize(responseContent);

                log1.ResponseTimestamp = DateTime.Now;
                log1.ResponseContentType = "application/json";
                
                var config2 = new MapperConfiguration(cfg =>
                {
                    //Model to DTO
                    cfg.CreateMap<LogMetadata, Apilog>();
                });

                IMapper iMapper2 = config2.CreateMapper();

                Apilog aPILog1 = iMapper2.Map<LogMetadata, Apilog>(log1);

                _context.Entry(aPILog1).State = EntityState.Added;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                string argmessage = ex.Message;
                if (ex.InnerException != null)
                {
                    argmessage += ex.InnerException;
                }
                throw ex.InnerException;
            }

        }
    }
}
