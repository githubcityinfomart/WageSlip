using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Static;

namespace MyPaySlipLive.BLL.Interface
{
    public interface IEmployee
    {
        Task<Response> AddEmployeeDetails(AddUpdateEmployeeDto userCredentials, string loginCompanyCode);
        Task<Response> Update(EmployeeDto updatedEmployeeData);
        Task<Response> GetAll(GetAllCompanyEmployeeDto getAllCompanyEmployeeDto);
        Task<Response> GetById(Guid id);
        Task<Response> GetAllByCompany(GetAllCompanyEmployeeDto getAllCompanyEmployeeDto, string searchString);

    }
}
