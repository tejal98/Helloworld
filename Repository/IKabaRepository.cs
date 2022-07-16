using KabaLockIntegration.Models;

namespace KabaLockIntegration.Repository
{
    public interface IKabaRepository
    {
        BaseResponse<OTC> GenerateOTC(KabaRequestModel kabaRequest);
        BaseResponse<CloseSealResponse> CloseSeal(DecodeCloseSealModel kabaRequest);
    }
}
