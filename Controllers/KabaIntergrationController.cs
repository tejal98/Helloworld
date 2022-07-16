using KabaLockIntegration.Models;
using KabaLockIntegration.Repository;
using KabaLockIntegration.RepositoryImplementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KabaLockIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KabaIntergrationController : ControllerBase
    {
        private readonly KabaDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public KabaIntergrationController(KabaDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("GenerateOTC")]
        public BaseResponse<OTC> GetOTC (KabaRequestModel kabaRequest)
        {
            IKabaRepository kabaRepository = new KabaRepositoryImplementation(_dbContext,_configuration);

            BaseResponse<OTC> res = kabaRepository.GenerateOTC(kabaRequest);
            return res;
        }

        [HttpPost("CloseSeal")]
        public BaseResponse<CloseSealResponse> CloseSeal(DecodeCloseSealModel kabaRequest)
        {
            IKabaRepository kabaRepository = new KabaRepositoryImplementation(_dbContext, _configuration);

            BaseResponse<CloseSealResponse> res = kabaRepository.CloseSeal(kabaRequest);
            return res;
        }
    }
}
