using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Static;
using System.ComponentModel.Design;
using System.Net;
using System.Text.Json.Serialization;

namespace MyPaySlipLive.BLL.Service
{
	public class UserService : IUser
	{
		private readonly IJwt _jwt;
		private readonly PayslipDbContext _db;
		private readonly IConfiguration _configuration;

		public UserService(IJwt jwt, PayslipDbContext dbContext, IConfiguration configuration)
		{
			_jwt = jwt;
			_db = dbContext;
			_configuration = configuration;
		}

		public async Task<Response> GetByCode(string eCode, string companyCode)
		{
			var company = _db.TblUsers.FirstOrDefault(e => e.CompanyCode == companyCode && e.Role == 2); // 2 role for company

			var employee = await (from u in _db.TblUsers
								  where u.UserCode == eCode && u.CompanyCode == companyCode
								  join ud in _db.TblUserDetails on new { userCode = u.UserCode, userId = u.Id } equals new { userCode = ud.Ecode, userId = ud.UserId }
								  select new
								  {
									  Ecode = ud.Ecode,
									  Designation = ud.Category,
									  Name = ud.Name,
									  Wdays = ud.Wdays,
									  SapCode = ud.SapCode,
									  Ttl = ud.Ttl,
									  NhLv = ud.Nhlv,
									  Others = ud.Others,
									  Conv = ud.Conv,
									  Tds = ud.Tds,
									  TransferredToAc = ud.TransferredToAc,

									  Leaves = ud.Leaves,
									  Comm = ud.Comm,
									  Advance = ud.Advance,
									  Tax = ud.Tax,
									  Month = ud.Month,
									  Year = ud.Year,
									  Company = ud.Company,
									  Location = ud.Location,
									  Sex = ud.Sex,
									  Chq = ud.Chq,
									  Bank = ud.Bank,
									  Account = ud.Account,
									  PfNumber = ud.PfNumber,
									  EsiNumber = ud.EsiNumber,
									  Category = ud.Category,
									  SalBasis = ud.SalBasis,
									  Basic = ud.Basic,
									  Hra = ud.Hra,
									  Ca = ud.Ca,
									  Allow = ud.Allow,
									  Washing = ud.Washing,
									  Total = ud.Total,
									  Ebasic = ud.Ebasic,
									  Ehra = ud.Ehra,
									  Eca = ud.Eca,
									  Eallow = ud.Eallow,
									  ReImb = ud.ReImb,
									  Etotal = ud.Etotal,
									  Pf = ud.Pf,
									  Esi = ud.Esi,
									  Tded = ud.Tded,
									  NetInr = ud.Net,
									  Fpf = ud.Fpf,
									  Epf = ud.Epf,
									  PfEmp = ud.PfEmp,
									  Esie = ud.Esie,
									  Gross = ud.Gross,
									  Remark = ud.Remark,
                                      MobileNumber = ud.MobileNumber,
									  CompanyCode = u.CompanyCode
								  }).Distinct().ToListAsync();
			return new Response(HttpStatusCode.OK.ToString(), $"{Message.Found}", JsonConvert.SerializeObject(employee));
		}

		public async Task<Response> Login(LoginDto userCredentials)
		{
			try
			{
				TblUser userDetail;
				string companyName = "";
				string companyCode = userCredentials?.CompanyCode?.Trim()!;
				if (!string.IsNullOrEmpty(companyCode)) // Login for user or admin
				{
					if (userCredentials!.Username.ToLower() == "admin") // Login for Admin 
					{
						userDetail = _db.TblUsers.FirstOrDefault(e => e.Password == userCredentials.Password && e.CompanyCode == companyCode && e.Role == 2)!;
					}
					else // Login for User 
					{
						userDetail = _db.TblUsers.FirstOrDefault(e => e.UserCode!.ToLower() == userCredentials!.Username.ToLower() && e.Password == userCredentials.Password && e.CompanyCode == companyCode)!;
					}
					//userDetail = _db.TblUsers.FirstOrDefault(e => (e.UserCode!.ToLower() == userCredentials!.Username.ToLower() || e.Name!.ToLower() == userCredentials!.Username.ToLower()) && e.Password == userCredentials.Password && e.CompanyCode == companyCode)!;

					companyName = _db.TblUsers.FirstOrDefault(e => e.CompanyCode == companyCode && e.Role == 2)?.Name!;
				}
				else { userDetail = _db.TblUsers.FirstOrDefault(e => e.Name.ToLower() == userCredentials!.Username.ToLower() && e.Password == userCredentials.Password)!; } // Superadmin login

				if (userDetail != null)
				{
					var role = _db.TblRoles.FirstOrDefault(e => e.Id == userDetail.Role)?.Role;
					var generateJwt = new GenerateJwtDto() { Name = userDetail.Name, Role = role!, CompanyCode = userDetail.CompanyCode!, EmployeeCode = userDetail.UserCode!, CompanyName = companyName };
					var jwtToken = await _jwt.GenerateJWToken(generateJwt, _configuration);
					return new Response(HttpStatusCode.OK.ToString(), "User is valid", JsonConvert.SerializeObject(jwtToken));
				}
				return new Response(HttpStatusCode.BadRequest.ToString(), "User credentials are invalid");
			}
			catch
			{
				return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.SomethingWentWrong}");
			}
		}

		public async Task<Response> ResetPassword(ResetPasswordDto resetPasswordDto)
		{
			try
			{
				var user = new TblUser();
 				if (string.IsNullOrEmpty(resetPasswordDto.NewPassword)) return new Response(HttpStatusCode.BadRequest.ToString(), "Password is necessary");

				if (!string.IsNullOrEmpty(resetPasswordDto.Token)) user = await _jwt.GetUserDataFromJWT(resetPasswordDto.Token);
				else user = _db.TblUsers.FirstOrDefault(e => e.Id == resetPasswordDto.EmployeeId);

				if (user == null) return new Response(HttpStatusCode.BadRequest.ToString(), $"User is invalid");

				if (resetPasswordDto.NewPassword == user!.Password) return new Response(HttpStatusCode.BadRequest.ToString(), "The new password can't be the same as the old password");

				user.Password = resetPasswordDto.NewPassword;
				user.PhoneNumber = resetPasswordDto.PhoneNumber;
 				_db.TblUsers.Update(user);

				_db.SaveChanges();
				return new Response(HttpStatusCode.OK.ToString(), $"Password {Message.UpdateSuccessfully}");
			}
			catch { return new Response(HttpStatusCode.BadRequest.ToString(), $"Password {Message.UpdateFail}"); }
		}

		public async Task<Response> ResetToDefaultPassword(ResetPasswordDto resetPasswordDto)
		{
			try
			{
				var user = new TblUser();
				user = _db.TblUsers.FirstOrDefault(e => e.Id == resetPasswordDto.EmployeeId);
				if (user == null) return new Response(HttpStatusCode.BadRequest.ToString(), $"User is invalid");
				string namePart = user.Name!.Length >= 3 ? user.Name.Substring(0, 3) : user.Name;
				string ecodePart = user.UserCode!.Length >= 3 ? user.UserCode.Substring(0, 3) : user.UserCode;
				string password = ecodePart + namePart.ToLower();
				user.Password = password;
				_db.TblUsers.Update(user);
				_db.SaveChanges();
				return new Response(HttpStatusCode.OK.ToString(), $"Password {Message.UpdateSuccessfully} To Default.");
			}
			catch { return new Response(HttpStatusCode.BadRequest.ToString(), $"Password {Message.UpdateFail}"); }
		}
	}
}
