using MyPaySlipLive.DAL;
using MyPaySlipLive.Models.Enum;
using MyPaySlipLive.Models.Static;
using System.Dynamic;
using System.Xml.Linq;

namespace MyPaySlipLive.BLL.AddDefaultData
{
    public class AddDefaultData
    {

        public static bool CheckIsRoleAvailable()
        {
            using (var dbContext = new PayslipDbContext())
            {
                return dbContext.TblRoles.ToList().Count() > 0;
            }
        }

        public static async Task AddDefaultRoles()
        {
            using (var _db = new PayslipDbContext())
            {
                var rolesWithIds = new Dictionary<Role, int>
                    {
                        { Role.SuperAdmin, 1 },
                        { Role.Admin, 2 },
                        { Role.User, 3 }
                    };
                foreach (var roleWithId in rolesWithIds)
                {
                    var roleExists = _db.TblRoles.Any(r => r.Role == roleWithId.Key.ToString());
                    if (!roleExists)
                    {
                        var newRole = new TblRole
                        {
                            Id = roleWithId.Value,  // Assign the specific ID
                            Role = roleWithId.Key.ToString(),
                            Date = GetTime.GetIndianTimeZone()
                        };
                        _db.TblRoles.Add(newRole);
                    }
                }

                await _db.SaveChangesAsync();
            }
        }


        public static async Task AddDefaultSuperAdmin()
        {
            try
            {
                using (var dbContext = new PayslipDbContext())
                {
                    if (!dbContext.TblUsers.Any(e => e.Role == 1)) // Role 1 will always be for superadmin
                    {
                        
                        var superAdmin = new TblUser()
                        {
                            
                            Name = "superadmin",
                            Email = "superadmin@gmail.com",
                            Password = "123457",
                            Role = 1,
                            Date = GetTime.GetIndianTimeZone(),
                        };
                        dbContext.TblUsers.Add(superAdmin);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch
            {

            }
        }
    }
}
