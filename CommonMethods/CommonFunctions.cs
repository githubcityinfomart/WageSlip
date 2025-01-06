using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyPaySlipLive.CommonMethods
{
    public class CommonFunctions
    {
        public static async Task<List<string>> DecodeJwt(string token)
        {
            var userRoles = new List<string>();

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var rolesClaim = jsonToken!.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            if (rolesClaim != null)
            {
                foreach (var role in rolesClaim)
                {
                    var roles = role.Value.Split(',');
                    userRoles.AddRange(roles);
                }
            }
            return userRoles;
        }
        public static async Task<string?> RetrieveCompanyCodeFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            var sid = jwtToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
            sid = string.IsNullOrEmpty(sid) ? "" : sid;
            return sid;
        }
        public static async Task<string?> RetrieveEmployeeCodeFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            var sid = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            sid = string.IsNullOrEmpty(sid) ? "" : sid;
            return sid;
        } 
        public static async Task<string?> RetrieveCompanyeNameFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            var sid = jwtToken.Claims.FirstOrDefault(c => c.Type == "CompanyName" || c.Type == "companyname")?.Value;
            sid = string.IsNullOrEmpty(sid) ? "" : sid;
            return sid;
        }
    }
}
