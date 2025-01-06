using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;

namespace MyPaySlipLive.BLL.Interface
{
    public interface IJwt
    {
         Task<string> GenerateJWToken(GenerateJwtDto userCredentials, IConfiguration configuration);
        Task<TblUser> GetUserDataFromJWT(string jwtToken);
    }
}
