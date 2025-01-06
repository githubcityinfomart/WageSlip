using Microsoft.EntityFrameworkCore;
using MyPaySlipLive.BLL.Interface;
using Newtonsoft.Json;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Static;
using System.Net;
using System.Xml.Linq;
using MyPaySlipLive.Models.AdminModel;
using MyPaySlipLive.Models.Enum;

namespace MyPaySlipLive.BLL.Service
{
    public class CompanyService : ICompany
    {
        private readonly PayslipDbContext _db;
        public CompanyService(PayslipDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Response> Add(CompanyDto companyDetail)
        {
            try
            {
                if (_db.TblUsers.Any(e => e.CompanyCode == companyDetail.Code && e.Name == companyDetail.Name))
                {
                    return new Response(HttpStatusCode.BadRequest.ToString(), $"Company code and name {Message.AlreadyExist}");
                }

                var newCompany = new TblUser()
                {
                    Name = companyDetail.Name,
                    CompanyCode = companyDetail.Code,
                    Email = companyDetail.Email,
                    Password = companyDetail.Password,
                    Role = 2,
                    IsPayed = true,
                    //  IsActive = true,
                    Date = GetTime.GetIndianTimeZone()
                };
                _db.TblUsers.Add(newCompany);
                await _db.SaveChangesAsync();
                return new Response(HttpStatusCode.OK.ToString(), $"{Message.AddSuccessfully}");
            }
            catch { return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.AddFail}"); }

        }
        public async Task<Response> Update(CompanyDto companyDetail)
        {
            try
            {
                var company = _db.TblUsers.FirstOrDefault(e => e.Id == companyDetail.Id);
                if (company == null)
                {
                    return new Response(HttpStatusCode.BadRequest.ToString(), $"Comapny {Message.NotFound}");
                }

                if (_db.TblUsers.Any(e => e.CompanyCode == companyDetail.Code && e.Name == companyDetail.Name && e.Id != companyDetail.Id))
                {
                    return new Response(HttpStatusCode.BadRequest.ToString(), $"Company code {Message.AlreadyExist}");
                }
                if (companyDetail.Email != null)
                {
                    company.Email = string.IsNullOrEmpty(companyDetail.Email.Trim()) ? company.Email : companyDetail.Email;
                }
                company.Name = string.IsNullOrEmpty(companyDetail.Name.Trim()) ? company.Name : companyDetail.Name;
                company.Password = string.IsNullOrEmpty(companyDetail.Password.Trim()) ? company.Password : companyDetail.Password;
                company.CompanyCode = string.IsNullOrEmpty(companyDetail.Code.Trim()) ? company.CompanyCode : companyDetail.Code;
                company.IsActive = companyDetail.IsActive;
                _db.TblUsers.Update(company);
                await _db.SaveChangesAsync();
                return new Response(HttpStatusCode.OK.ToString(), $"{Message.UpdateSuccessfully}");
            }
            catch
            {
                return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.UpdateFail}");
            }
        }

        public async Task<Response> AddUpdate(CompanyDto companyDetail)
        {
            if (companyDetail.Id == Guid.Empty) { return await Add(companyDetail); }
            return await Update(companyDetail);
        }

        public async Task<Response> GetAll(Pagination pagination)
        {
            pagination.PageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
            pagination.PageSize = pagination.PageSize > 0 ? pagination.PageSize : 20;
            int skip = (pagination.PageNumber - 1) * pagination.PageSize;
            var companyList = await _db.TblUsers.Where(e => e.Role == 2).OrderByDescending(e => e.Date).ToListAsync(); // Role 2 for company
            var companyListAfterPagination = companyList.Skip(skip).Take(pagination.PageSize);
            return new Response(HttpStatusCode.OK.ToString(), $"{companyList.Count}{Message.Found}", companyList.Count(), JsonConvert.SerializeObject(companyListAfterPagination));
        }

        public async Task<Response> GetById(Guid id)
        {
            var companyDetail = await _db.TblUsers.FirstOrDefaultAsync(e => e.Id == id);
            if (companyDetail == null) { return new Response(HttpStatusCode.BadRequest.ToString(), $"Company {Message.NotFound}"); }
            return new Response(HttpStatusCode.OK.ToString(), $"Company {Message.Found}", JsonConvert.SerializeObject(companyDetail));
        }


        public async Task<Response> GetAllCompaniesAndCompanyCode()
        {
            try
            {

                var companyCodes = await _db.TblUsers
                    .Where(e => e.Role == 2)
                    .OrderByDescending(e => e.Date)
                    .Select(e => e.CompanyCode)
                    .ToListAsync();

                var companyNames = await _db.TblUserDetails
                    .Where(e => !string.IsNullOrEmpty(e.Company))
                    .Select(e => e.Company).Distinct()
                    .ToListAsync();

                var data = new ExportSlipViewModel
                {
                    CompanyCodes = companyCodes.Select(code => new GetComapanyCode { CompanyCodes = code }).ToList(),
                    CompanyNames = companyNames.Select(name => new GetCompanyName { CompanyNames = name }).ToList()
                };
                return new Response(HttpStatusCode.OK.ToString(), $"Company {Message.Found}", JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {

                return new Response(HttpStatusCode.InternalServerError.ToString(), $"An error occurred: {ex.Message}");
            }
        }


        public async Task<Response> GetByFilter(string companyCode, string? companyName, string? month, string? ecode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return new Response(HttpStatusCode.BadRequest.ToString(), $"Please Select Company Code.");
                }
                var query = from user in _db.TblUsers
                            join detail in _db.TblUserDetails on user.Id equals detail.UserId
                            where user.CompanyCode == companyCode
                                  && (string.IsNullOrEmpty(companyName) || detail.Company == companyName)
                                  && (string.IsNullOrEmpty(month) || detail.Month == month)
                                  && (string.IsNullOrEmpty(ecode) || detail.Ecode == ecode)
                            select new UserDetailViewModel
                            {
                                Id = detail.Id,
                                UserId = user.Id,
                                Ecode = detail.Ecode ?? string.Empty,
                                Designation = detail.Category ?? string.Empty,
                                Name = detail.Name ?? string.Empty,
                                Wdays = detail.Wdays ?? "0",
                                SapCode = detail.SapCode ?? string.Empty,
                                Ttl = detail.Ttl ?? "0",
                                NhLv = detail.Nhlv ?? "0",
                                Others = detail.Others ?? "0",
                                Conv = detail.Conv ?? "0",
                                Tds = detail.Tds ?? "0",
                                TransferredToAc = detail.TransferredToAc ?? string.Empty,
                                Leaves = detail.Leaves ?? "0",
                                Comm = detail.Comm ?? "0",
                                Advance = detail.Advance ?? "0",
                                Tax = detail.Tax ?? "0",
                                Month = detail.Month ?? string.Empty,
                                Year = detail.Year ?? string.Empty,
                                Company = detail.Company ?? string.Empty,
                                Location = detail.Location ?? string.Empty,
                                Sex = detail.Sex ?? string.Empty,
                                Chq = detail.Chq ?? string.Empty,
                                Bank = detail.Bank ?? string.Empty,
                                Account = detail.Account ?? string.Empty,
                                PfNumber = detail.PfNumber ?? string.Empty,
                                EsiNumber = detail.EsiNumber ?? string.Empty,
                                Category = detail.Category ?? string.Empty,
                                SalBasis = detail.SalBasis ?? string.Empty,
                                Basic = detail.Basic ?? "0",
                                Hra = detail.Hra ?? "0",
                                Ca = detail.Ca ?? "0",
                                Allow = detail.Allow ?? "0",
                                Washing = detail.Washing ?? "0",
                                Total = detail.Total ?? "0",
                                Ebasic = detail.Ebasic ?? "0",
                                Ehra = detail.Ehra ?? "0",
                                Eca = detail.Eca ?? "0",
                                Eallow = detail.Eallow ?? "0",
                                ReImb = detail.ReImb ?? "0",
                                Etotal = detail.Etotal ?? "0",
                                Pf = detail.Pf ?? "0",
                                Esi = detail.Esi ?? "0",
                                Tded = detail.Tded ?? "0",
                                NetInr = detail.Net ?? "0",
                                Fpf = detail.Fpf ?? "0",
                                Epf = detail.Epf ?? "0",
                                PfEmp = detail.PfEmp ?? "0",
                                Esie = detail.Esie ?? "0",
                                Gross = detail.Gross ?? "0",
                                Remark = detail.Remark ?? string.Empty,
                                MainCompanyName = _db.TblUsers
                                                  .Where(c => c.CompanyCode == companyCode && c.Role == 2)
                                                  .Select(c => c.Name)
                                                  .FirstOrDefault()
                            };

                var employeeList = await query.ToListAsync();
 

                if (!employeeList.Any())
                {
                    return new Response(HttpStatusCode.NotFound.ToString(), $" Data {Message.NotFound}");
                }

                var exportSlipViewModel = new ExportSlipViewModel
                {
                    UserDetail = employeeList
                };

                return new Response(HttpStatusCode.OK.ToString(), $"{Message.Found}", JsonConvert.SerializeObject(exportSlipViewModel));
            }
            catch (Exception ex)
            {
                return new Response(HttpStatusCode.InternalServerError.ToString(), $"An error occurred: {ex.Message}");
            }

        }


    }


}

