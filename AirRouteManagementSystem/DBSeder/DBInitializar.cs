using AirRouteManagementSystem.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.DBSeder
{
    public class DBInitializar : IDBInitializar
    {
        private readonly ApplicationDBContext _dBContext;
        private readonly ILogger<DBInitializar> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializar(ApplicationDBContext dBContext, ILogger<DBInitializar> logger,
            RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            _dBContext = dBContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void  Initialiaze()
        {
            try
            {
                if (_dBContext.Database.GetPendingMigrations().Any())
                {
                    _dBContext.Database.Migrate();
                }

                if(!_roleManager.RoleExistsAsync(SD.SuperAdminRole).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new(SD.SuperAdminRole)).GetAwaiter().GetResult();
                }
                if(!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new(SD.AdminRole)).GetAwaiter().GetResult();
                }
                if(!_roleManager.RoleExistsAsync(SD.EmployeeRole).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new(SD.EmployeeRole)).GetAwaiter().GetResult();
                }
                if(!_roleManager.RoleExistsAsync(SD.CustomerRole).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new(SD.CustomerRole)).GetAwaiter().GetResult();
                }
           
                      _userManager.CreateAsync(new()
                    {
                        UserName="SuperAdmin",
                        Email="SuperAdmin@gmail.com",
                        Name="SuperAdmin",
                        Address="Helwan",
                        EmailConfirmed=true,
                    }, "AdminAdmin123#").GetAwaiter().GetResult();

                    var user=_userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();

                     _userManager.AddToRoleAsync(user!, SD.SuperAdminRole).GetAwaiter().GetResult();
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {ex.Message}");
            }
        }
    }
}
