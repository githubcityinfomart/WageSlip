using MyPaySlipLive.Models;

namespace MyPaySlipLive.BLL.Interface
{
    public interface ICompany
    {
        Task<Response> AddUpdate(CompanyDto companyDetail);
        Task<Response> GetAll(Pagination pagination);
        Task<Response> GetById(Guid id);
         Task<Response> GetAllCompaniesAndCompanyCode();
        Task<Response> GetByFilter( string companyCode, string companyName, string month , string ecode);
    }
}
