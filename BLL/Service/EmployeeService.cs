using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Static;
using System.Net;

namespace MyPaySlipLive.BLL.Service
{
    public class EmployeeService : IEmployee
    {
        private readonly PayslipDbContext _db;
        public EmployeeService(PayslipDbContext db)
        {
            _db = db;
        }
        public async Task<Response> AddEmployeeDetails(AddUpdateEmployeeDto employeeData, string loginCompanyCode)
        {
            // Start a transaction
            using (var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {

                    var company = _db.TblUsers.FirstOrDefault(e => e.Role == 2 && e.CompanyCode == loginCompanyCode)!;
                    foreach (var data in employeeData.EmployeeData)
                    {
					
 						bool userExist = _db.TblUsers
						   .FirstOrDefault(e => e.Role == 3
											 && e.CompanyCode == company.CompanyCode
											 && e.UserCode == data.Ecode) != null;
 
                        data.Month = data.Date.Month.ToString();
                        data.Year = data.Date.Year.ToString();
                       
                        var employeeOldData = await _db.TblUserDetails.FirstOrDefaultAsync(e => e.Ecode == data.Ecode && e.Month == data.Month.ToString() && e.Year == data.Year.ToString() && e.Company == data.Company );
                        if (employeeOldData != null && userExist)
                        {
                            data.UserId = employeeOldData.UserId;
                            await UpdateEmployeeDetail(data);
                        }
                        else
                        {
                            var employee = _db.TblUsers.FirstOrDefault(e => e.UserCode == data.Ecode && e.CompanyCode == company.CompanyCode); // 
							data.CompanyCode = loginCompanyCode;
							if (employee == null) {  data.UserId = await AddEmployee(data); }
                            else { data.UserId = employee!.Id; }
                            if (data.UserId != Guid.Empty) await AddEmployeeDetail(data);
                        }
                    }
                    foreach (var data in employeeData.EmployeeData)
                    {
                        if (await IsUserRecordMoreThan12((Guid)data.UserId!))
                        {
                            await RemoveEmployeeExsesiveRecords((Guid)data.UserId!);
                        }
                    }
                    await transaction.CommitAsync();
                    return new Response(HttpStatusCode.OK.ToString(), $"{Message.AddSuccessfully}");
                }
                catch (Exception e)
                {  // Rollback a transaction
                    await transaction.RollbackAsync();
                }
            }
            return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.SomethingWentWrong}");
        }
        public async Task<Guid> AddEmployee(EmployeeDto newEmployeeData) // Add Employee data in master table
        {

            try
            {
				var company = _db.TblUsers.FirstOrDefault(e => e.CompanyCode == newEmployeeData.CompanyCode && e.Role == 2);
				//var company = _db.TblUsers.FirstOrDefault(e => e.Name == newEmployeeData.Company && e.Role == 2);
                if (company != null) // Is Company regirsted in our db
                {
                    if (!_db.TblUsers.Any(e => e.Name == newEmployeeData.Name && e.CompanyCode == newEmployeeData.CompanyCode && e.UserCode == newEmployeeData.Ecode)) // First check for user and second for company
                    {

                        string namePart = newEmployeeData.Name!.Length >= 3 ? newEmployeeData.Name.Substring(0, 3) : newEmployeeData.Name;
                        string ecodePart = newEmployeeData.Ecode.Length >= 3 ? newEmployeeData.Ecode.Substring(0, 3) : newEmployeeData.Ecode;
                        string password = ecodePart + namePart.ToLower();

                        var employee = new TblUser()
                        {
                            Name = newEmployeeData.Name!,
                            Email = "",
                            Role = 3,
                            UserCode = newEmployeeData.Ecode,
                            CompanyCode = company.CompanyCode,
                            Password = password,
                            IsActive = true,
                            Date = GetTime.GetIndianTimeZone(),
                            IsPayed = false,
                        };
                        _db.TblUsers.Add(employee);
                        await _db.SaveChangesAsync();
                        return employee.Id;
                    }
                }
            }
            catch { }
            return Guid.Empty;
        }
        public async Task<bool> AddEmployeeDetail(EmployeeDto updatedEmployeeData) // Add emplyee data in detail table
        {

            try
            {
                var employeeData = new TblUserDetail();
                {
                    employeeData.Ecode = updatedEmployeeData.Ecode ?? string.Empty;
                    employeeData.Name = updatedEmployeeData.Name ?? string.Empty;
                    employeeData.Month = updatedEmployeeData.Month?.ToString() ?? string.Empty;
                    employeeData.Year = updatedEmployeeData.Year?.ToString() ?? string.Empty;
                    employeeData.Wdays = updatedEmployeeData.Wdays?.ToString() ?? "0";
                    employeeData.Leaves = updatedEmployeeData.Leaves?.ToString() ?? "0";
                    employeeData.Comm = updatedEmployeeData.Comm ?? "0";  // Assuming Comm is a number, use default 0
                    employeeData.Advance = updatedEmployeeData.Advance?.ToString() ?? "0";
                    employeeData.Tax = updatedEmployeeData.Tax?.ToString() ?? "0";
                    employeeData.Company = updatedEmployeeData.Company ?? string.Empty;
                    employeeData.Location = updatedEmployeeData.Location ?? string.Empty;
                    employeeData.Sex = updatedEmployeeData.Sex ?? string.Empty;
                    employeeData.Chq = updatedEmployeeData.Chq ?? string.Empty;
                    employeeData.Bank = updatedEmployeeData.Bank ?? string.Empty;
                    employeeData.Account = updatedEmployeeData.Account?.ToString() ?? string.Empty;
                    employeeData.PfNumber = updatedEmployeeData.PfNumber?.ToString() ?? string.Empty;
                     employeeData.EsiNumber = updatedEmployeeData.EsiNumber ?? string.Empty;
                    employeeData.Category = updatedEmployeeData.Category ?? string.Empty;
                    employeeData.SalBasis = updatedEmployeeData.SalBasis ?? string.Empty;
                    employeeData.Basic = updatedEmployeeData.Basic?.ToString() ?? "0";
                    employeeData.Hra = updatedEmployeeData.Hra?.ToString() ?? "0";
                    employeeData.Ca = updatedEmployeeData.Ca?.ToString() ?? "0";
                    employeeData.Allow = updatedEmployeeData.Allow?.ToString() ?? "0";
                    employeeData.Washing = updatedEmployeeData.Washing ?? "0";  // Assuming Washing is a number, use default 0
                    employeeData.Total = updatedEmployeeData.Total?.ToString() ?? "0";
                    employeeData.Ebasic = updatedEmployeeData.Ebasic?.ToString() ?? "0";
                    employeeData.Ehra = updatedEmployeeData.Ehra?.ToString() ?? "0";
                    employeeData.Eca = updatedEmployeeData.Eca?.ToString() ?? "0";
                    employeeData.Eallow = updatedEmployeeData.Eallow?.ToString() ?? "0";
                    employeeData.ReImb = updatedEmployeeData.ReImb?.ToString() ?? "0";
                    employeeData.Etotal = updatedEmployeeData.Etotal?.ToString() ?? "0";
                    employeeData.Pf = updatedEmployeeData.Pf?.ToString() ?? "0";
                    employeeData.Esi = updatedEmployeeData.Esi?.ToString() ?? "0";
                    employeeData.Tded = updatedEmployeeData.Tded?.ToString() ?? "0";
                    employeeData.Net = updatedEmployeeData.Net?.ToString() ?? "0";
                    employeeData.Fpf = updatedEmployeeData.Fpf?.ToString() ?? "0";
                    employeeData.Epf = updatedEmployeeData.Epf?.ToString() ?? "0";
                    employeeData.PfEmp = updatedEmployeeData.PfEmp?.ToString() ?? "0";
                    employeeData.Esie = updatedEmployeeData.Esie?.ToString() ?? "0";
                    employeeData.Gross = updatedEmployeeData.Gross?.ToString() ?? "0";
                    employeeData.Remark = updatedEmployeeData.Remark ?? string.Empty;


                    employeeData.SapCode = updatedEmployeeData.SapCode ?? string.Empty;
                    employeeData.Ttl = updatedEmployeeData.Ttl ?? string.Empty;
                    employeeData.Nhlv = updatedEmployeeData.Nhlv ?? string.Empty;
                    employeeData.Others = updatedEmployeeData.Others ?? string.Empty;
                    employeeData.Conv = updatedEmployeeData.Conv ?? string.Empty;
                    employeeData.Tds = updatedEmployeeData.Tds ?? string.Empty;
                    employeeData.TransferredToAc = updatedEmployeeData.TransferredToAc ?? string.Empty;
                    employeeData.MobileNumber = updatedEmployeeData.MobileNumber ?? string.Empty;
                    employeeData.TotalDeduction = updatedEmployeeData.TotalDeduction ?? string.Empty;
                   

                    employeeData.UserId = updatedEmployeeData.UserId ?? Guid.Empty; // Use Guid.Empty if UserId is null
                    _db.TblUserDetails.Add(employeeData);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e) { }
            return false;
        }
        public async Task<bool> IsUserRecordMoreThan12(Guid employeeId)
        {
            return await _db.TblUserDetails.Where(e => e.UserId == employeeId).CountAsync() > 12;
        }
        public async Task RemoveEmployeeExsesiveRecords(Guid employeeId)
        {
            try
            {
                var latest12Records = await _db.TblUserDetails.Where(e => e.UserId == employeeId).OrderByDescending(e => Convert.ToInt32(e.Year!)).ThenByDescending(e => Convert.ToInt32(e.Month!)).Take(12).ToListAsync(); // Get the latest 12 records based on Year and Month
                var latest12Ids = latest12Records.Select(e => e.Id).ToList();
                var recordsToDelete = _db.TblUserDetails.Where(e => e.UserId == employeeId && !latest12Ids.Contains(e.Id));
                _db.TblUserDetails.RemoveRange(recordsToDelete);
                await _db.SaveChangesAsync();
            }
            catch (Exception e) { }
        }
        public async Task<Response> Update(EmployeeDto updatedEmployeeData)
        {
            if (!_db.TblUserDetails.Any(e => (e.UserId == updatedEmployeeData.UserId || e.Ecode == updatedEmployeeData.Ecode) && e.Month == updatedEmployeeData.Month!.ToString() && e.Year == updatedEmployeeData.Year!.ToString()))
            {
                if (await UpdateEmployeeDetail(updatedEmployeeData))
                {
                    return new Response(HttpStatusCode.OK.ToString(), $"Employee data {Message.UpdateSuccessfully}");
                }
                return new Response(HttpStatusCode.BadRequest.ToString(), $"Employee data {Message.SomethingWentWrong}");
            }
            return new Response(HttpStatusCode.BadRequest.ToString(), $"Employee {Message.NotFound}");
        }
        public async Task<bool> UpdateEmployeeDetail(EmployeeDto updatedEmployeeData)
        {
            // Start a transaction

            {
                try
                {
                    var employeeData = await _db.TblUserDetails.FirstOrDefaultAsync(e => (e.UserId == updatedEmployeeData.UserId || e.Ecode == updatedEmployeeData.Ecode) && e.Month == updatedEmployeeData.Month!.ToString() && e.Year == updatedEmployeeData.Year!.ToString());
                    if (employeeData != null)
                    {

                        //employeeData.Ecode = updatedEmployeeData.Ecode ?? string.Empty;
                        employeeData.Name = updatedEmployeeData.Name ?? string.Empty;
                        //employeeData.Month = updatedEmployeeData.Month?.ToString() ?? string.Empty;
                        //employeeData.Year = updatedEmployeeData.Year?.ToString() ?? string.Empty;
                        employeeData.Wdays = updatedEmployeeData.Wdays?.ToString() ?? "0";
                        employeeData.Leaves = updatedEmployeeData.Leaves?.ToString() ?? "0";
                        employeeData.Comm = updatedEmployeeData.Comm ?? "0";  // Assuming Comm is a number, use default 0
                        employeeData.Advance = updatedEmployeeData.Advance?.ToString() ?? "0";
                        employeeData.Tax = updatedEmployeeData.Tax?.ToString() ?? "0";
                        employeeData.Company = updatedEmployeeData.Company ?? string.Empty;
                        employeeData.Location = updatedEmployeeData.Location ?? string.Empty;
                        employeeData.Sex = updatedEmployeeData.Sex ?? string.Empty;
                        employeeData.Chq = updatedEmployeeData.Chq ?? string.Empty;
                        employeeData.Bank = updatedEmployeeData.Bank ?? string.Empty;
                        employeeData.Account = updatedEmployeeData.Account?.ToString() ?? string.Empty;
                        employeeData.PfNumber = updatedEmployeeData.PfNumber?.ToString() ?? string.Empty;
                         employeeData.EsiNumber = updatedEmployeeData.EsiNumber ?? string.Empty;
                        employeeData.Category = updatedEmployeeData.Category ?? string.Empty;
                        employeeData.SalBasis = updatedEmployeeData.SalBasis ?? string.Empty;
                        employeeData.Basic = updatedEmployeeData.Basic?.ToString() ?? "0";
                        employeeData.Hra = updatedEmployeeData.Hra?.ToString() ?? "0";
                        employeeData.Ca = updatedEmployeeData.Ca?.ToString() ?? "0";
                        employeeData.Allow = updatedEmployeeData.Allow?.ToString() ?? "0";
                        employeeData.Washing = updatedEmployeeData.Washing ?? "0";  // Assuming Washing is a number, use default 0
                        employeeData.Total = updatedEmployeeData.Total?.ToString() ?? "0";
                        employeeData.Ebasic = updatedEmployeeData.Ebasic?.ToString() ?? "0";
                        employeeData.Ehra = updatedEmployeeData.Ehra?.ToString() ?? "0";
                        employeeData.Eca = updatedEmployeeData.Eca?.ToString() ?? "0";
                        employeeData.Eallow = updatedEmployeeData.Eallow?.ToString() ?? "0";
                        employeeData.ReImb = updatedEmployeeData.ReImb?.ToString() ?? "0";
                        employeeData.Etotal = updatedEmployeeData.Etotal?.ToString() ?? "0";
                        employeeData.Pf = updatedEmployeeData.Pf?.ToString() ?? "0";
                        employeeData.Esi = updatedEmployeeData.Esi?.ToString() ?? "0";
                        employeeData.Tded = updatedEmployeeData.Tded?.ToString() ?? "0";
                        employeeData.Net = updatedEmployeeData.Net?.ToString() ?? "0";
                        employeeData.Fpf = updatedEmployeeData.Fpf?.ToString() ?? "0";
                        employeeData.Epf = updatedEmployeeData.Epf?.ToString() ?? "0";
                        employeeData.PfEmp = updatedEmployeeData.PfEmp?.ToString() ?? "0";
                        employeeData.Esie = updatedEmployeeData.Esie?.ToString() ?? "0";
                        employeeData.Gross = updatedEmployeeData.Gross?.ToString() ?? "0";
                        employeeData.Remark = updatedEmployeeData.Remark ?? string.Empty;

                        employeeData.SapCode = updatedEmployeeData.SapCode ?? string.Empty;
                        employeeData.Ttl = updatedEmployeeData.Ttl ?? string.Empty;
                        employeeData.Nhlv = updatedEmployeeData.Nhlv ?? string.Empty;
                        employeeData.Others = updatedEmployeeData.Others ?? string.Empty;
                        employeeData.Conv = updatedEmployeeData.Conv ?? string.Empty;
                        employeeData.Tds = updatedEmployeeData.Tds ?? string.Empty;
                        employeeData.TransferredToAc = updatedEmployeeData.TransferredToAc ?? string.Empty;
                        employeeData.MobileNumber = updatedEmployeeData.MobileNumber ?? string.Empty;
                        employeeData.TotalDeduction = updatedEmployeeData.TotalDeduction ?? string.Empty;


                        _db.TblUserDetails.Update(employeeData);
                        _db.SaveChanges();

                        return true;
                    }

                }
                catch
                {

                }
            }
            return false;
        }
        public async Task<Response> GetAll(GetAllCompanyEmployeeDto getAllCompanyEmployeeDto)
        {
            getAllCompanyEmployeeDto.Pagination.PageNumber = getAllCompanyEmployeeDto.Pagination.PageNumber > 0 ? getAllCompanyEmployeeDto.Pagination.PageNumber : 1;
            getAllCompanyEmployeeDto.Pagination.PageSize = getAllCompanyEmployeeDto.Pagination.PageSize > 0 ? getAllCompanyEmployeeDto.Pagination.PageSize : 20;
            int skip = (getAllCompanyEmployeeDto.Pagination.PageNumber - 1) * getAllCompanyEmployeeDto.Pagination.PageSize;
            var employees = await (from u in _db.TblUsers
                                   where u.CompanyCode == getAllCompanyEmployeeDto.CompanyCode || u.Name == getAllCompanyEmployeeDto.CompanyCode
                                   join ud in _db.TblUserDetails
                                  on u.Id equals ud.UserId
                                   select new { ud , CompanyCode = u.CompanyCode }).ToListAsync();
            var employeeListAfterPagination = employees.Skip(skip).Take(getAllCompanyEmployeeDto.Pagination.PageSize);
            return new Response(HttpStatusCode.OK.ToString(), $"{Message.Found}", employees.Count(), JsonConvert.SerializeObject(employeeListAfterPagination));
        }
        public async Task<Response> GetById(Guid id)
        {
            var employeeDetail = await _db.TblUserDetails.FirstOrDefaultAsync(e => e.UserId == id);

            if (employeeDetail == null) { return new Response(HttpStatusCode.BadRequest.ToString(), $"Company {Message.NotFound}"); }
            return new Response(HttpStatusCode.OK.ToString(), $"Company {Message.Found}", JsonConvert.SerializeObject(employeeDetail));
        }
        public async Task<Response> GetAllByCompany(GetAllCompanyEmployeeDto getAllCompanyEmployeeDto, string searchString)
        {
            var allEmployees = new List<TblUser>();
            var companyName = _db.TblUsers.FirstOrDefault(e => e.CompanyCode == getAllCompanyEmployeeDto.CompanyCode && e.Role == 2)?.Name;
            getAllCompanyEmployeeDto.Pagination.PageNumber = getAllCompanyEmployeeDto.Pagination.PageNumber > 0 ? getAllCompanyEmployeeDto.Pagination.PageNumber : 1;
            getAllCompanyEmployeeDto.Pagination.PageSize = getAllCompanyEmployeeDto.Pagination.PageSize > 0 ? getAllCompanyEmployeeDto.Pagination.PageSize : 20;
            int skip = (getAllCompanyEmployeeDto.Pagination.PageNumber - 1) * getAllCompanyEmployeeDto.Pagination.PageSize;
            if (!string.IsNullOrEmpty(searchString))
            {
                allEmployees = await _db.TblUsers.Where(e => e.Role == 3 && e.CompanyCode == getAllCompanyEmployeeDto.CompanyCode && e.Name!.Contains(searchString)).ToListAsync();
            }
            else
            {
                allEmployees = await _db.TblUsers.Where(e => e.Role == 3 && e.CompanyCode == getAllCompanyEmployeeDto.CompanyCode).ToListAsync();
            }
            //var filterData = (from allEmp in allEmployees
            //                  join ud in _db.TblUserDetails on allEmp.Id equals ud.UserId
            //                  select new
            //                  {
            //                      Id = allEmp.Id,
            //                      CompanyName = companyName,
            //                      Username = ud.Name,
            //                      EmailId = allEmp.Email,
            //                      Password = allEmp.Password,
            //                      Ecode = ud.Ecode,
            //                      MobileNumber = ud.MobileNumber
            //                  }).Skip(skip).Take(getAllCompanyEmployeeDto.Pagination.PageSize).ToList();
            var filterData = (from allEmp in allEmployees
                              join ud in _db.TblUserDetails on allEmp.Id equals ud.UserId
                              select new
                              {
                                  Id = allEmp.Id,
                                  CompanyName = companyName, // Ensure companyName is properly defined
                                  Username = ud.Name,
                                  EmailId = allEmp.Email,
                                  Password = allEmp.Password,
                                  Ecode = ud.Ecode,
                                  MobileNumber = allEmp.PhoneNumber,
                                  TotalDeduction = ud.TotalDeduction
                              }).DistinctBy(e => e.Ecode).Skip(skip).Take(getAllCompanyEmployeeDto.Pagination.PageSize).ToList();

            //var filterData = allEmployees.Select(e => new
            //{
            //    Id = e.Id,
            //    CompanyName = companyName,
            //    Username = e.Name,
            //    EmailId = e.Email,
            //    Password = e.Password,
            //    Ecode = e.UserCode
            //}).Skip(skip).Take(getAllCompanyEmployeeDto.Pagination.PageSize).ToList();
            return new Response(HttpStatusCode.OK.ToString(), $"{Message.Found}", allEmployees.Count(), JsonConvert.SerializeObject(filterData));

        }
    }
}