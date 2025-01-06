using MyPaySlipLive.Models;

namespace MyPaySlipLive.BLL.Interface
{
    public interface IUser
    {
        Task<Response> Login(LoginDto userCredentials);
        Task<Response> GetByCode(string eCode, string companyCode);
        Task<Response> ResetPassword(ResetPasswordDto resetPasswordDto); 
        Task<Response> ResetToDefaultPassword(ResetPasswordDto resetPasswordDto);

    }
}
