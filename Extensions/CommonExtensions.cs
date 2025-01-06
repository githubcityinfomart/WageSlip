using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPaySlipLive.Models;
using Newtonsoft.Json.Linq;
using MyPaySlipLive.CommonMethods;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MyPaySlipLive.Extensions
{
    [AllowAnonymous]
    public class CommonExtensions : PageModel
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommonExtensions(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> SignInUserAsync2(LoginDto model, Response response)
        {
            try
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
                var token = JsonConvert.DeserializeObject<string>(response.Result);
                var Role = await CommonFunctions.DecodeJwt(token!);
                var companycode = await CommonFunctions.RetrieveCompanyCodeFromJwt(token!);
                var firstRole = Role.FirstOrDefault();
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Username), new Claim(ClaimTypes.Role, firstRole!), new Claim("CompanyCode", companycode!) };
                claims.Add(new Claim(ClaimTypes.Dsa, response.Result));
                var handler = new JwtSecurityTokenHandler();
                var token1 = handler.ReadJwtToken(token);
                var expirationTimeUnix = token1.Payload.Exp;
                var expirationTimeUtc = DateTimeOffset.FromUnixTimeSeconds((long)expirationTimeUnix!)!.UtcDateTime;
                //foreach (var role in Role)
                //{
                //    claims.Add(new Claim("Role", role)); // Add the Role
                //}
                //claims.Add(new Claim("Name", username));
                //claims.Add(new Claim("Role", role));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = expirationTimeUtc.AddMinutes(-1)
                };

                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public async Task<bool> SignInUserAsync(LoginDto model, Response response)
        {
            try
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }

                var token = JsonConvert.DeserializeObject<string>(response.Result);
                var roles = await CommonFunctions.DecodeJwt(token!);
                var companyCode = await CommonFunctions.RetrieveCompanyCodeFromJwt(token!);
                var employeeCode = await CommonFunctions.RetrieveEmployeeCodeFromJwt(token!);
                var companyName = await CommonFunctions.RetrieveCompanyeNameFromJwt(token!);
                var firstRole = roles.FirstOrDefault() ?? "DefaultRole";

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Username),
            new Claim(ClaimTypes.Role, firstRole),
          new Claim("CompanyCode", companyCode!),
          new Claim("EmployeeCode", employeeCode!),
             new Claim("Token", token!),
             new Claim("CompanyName", companyName!)

        };

                claims.Add(new Claim("Token", token!));

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var expirationTimeUnix = jwtToken.Payload.Exp;
                var expirationTimeUtc = DateTimeOffset.FromUnixTimeSeconds((long)expirationTimeUnix!).UtcDateTime;

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = expirationTimeUtc.AddMinutes(-1)
                };

                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Logout()
        {
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return true;
                }
            }
            catch (Exception ex) { string error = ex.Message; }
            return false;

        }
    }
}
