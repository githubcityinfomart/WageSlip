using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Static;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyPaySlipLive.BLL.Service
{
    public class JwtService : IJwt
    {
        private readonly PayslipDbContext _context;
        public JwtService(PayslipDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateJWToken(GenerateJwtDto userCredentials, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            userCredentials.EmployeeCode = string.IsNullOrEmpty(userCredentials.EmployeeCode) ? "" : userCredentials.EmployeeCode;
            userCredentials.CompanyCode = string.IsNullOrEmpty(userCredentials.CompanyCode) ? "" : userCredentials.CompanyCode;
            userCredentials.Name = string.IsNullOrEmpty(userCredentials.Name) ? "" : userCredentials.Name;
            var claims = new List<Claim> {
                                            new Claim(JwtRegisteredClaimNames.UniqueName, userCredentials.Name),
                                            new Claim(JwtRegisteredClaimNames.Sid, userCredentials.CompanyCode),
                                            new Claim(JwtRegisteredClaimNames.NameId, userCredentials.EmployeeCode),
                                            new Claim("CompanyName", userCredentials.CompanyName),
                                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                                         };

            claims.Add(new Claim(ClaimTypes.Role, userCredentials.Role));
            var securityToken = new JwtSecurityToken(configuration["Jwt:Issuer"],
                                configuration["Jwt:Issuer"],
                                claims,
                                expires: GetTime.GetIndianTimeZone().AddDays(1),
                                signingCredentials: credentials);
            var jwtHandler = new JwtSecurityTokenHandler();
            return jwtHandler.WriteToken(securityToken);

        }
        public async Task<TblUser> GetUserDataFromJWT(string jwtToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtToken);

                // Extract the Name and Role from the JWT claims
                Claim nameClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)!;
                Claim employeeCode = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)!;
                Claim roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!;
                Claim companyCodeClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)!;

<<<<<<< HEAD
                if ( roleClaim == null)
=======
                if (roleClaim == null)
>>>>>>> 015c68a791fdcb46e0daaee858d7e4494ebb48f8
                {
                    return null!;
                }
                string name = nameClaim.Value ?? string.Empty;
                string userCode = employeeCode.Value ?? string.Empty;
                string companyCode = companyCodeClaim.Value ?? string.Empty;
<<<<<<< HEAD
 
=======
>>>>>>> 015c68a791fdcb46e0daaee858d7e4494ebb48f8

                int roleId = 0;
                var roles = _context.TblRoles.ToList();
                foreach (var role in roles)
                {
                    if (role.Role == roleClaim.Value)
                    {
                        roleId = role.Id;
                        break;
                    }
                }
                TblUser? user = null;
                if (roleId == 1)
                {
                    user = await _context.TblUsers.FirstOrDefaultAsync(e => e.Name == name && e.Role == roleId);
                }
                else if (roleId == 2)
                {
                    user = await _context.TblUsers.FirstOrDefaultAsync(e => e.CompanyCode == companyCode && e.Role == roleId && e.Name == name);
                }
                else if (roleId == 3)
                {
                    user = await _context.TblUsers.FirstOrDefaultAsync(e => e.CompanyCode == companyCode && e.Role == roleId && e.UserCode == userCode);
                }
                return user ?? null!;
            }
            catch
            {
                return null!;
            }
        }

    }
}
