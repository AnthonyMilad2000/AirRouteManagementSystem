using AirRouteManagementSystem.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AirRouteManagementSystem.DateAccess.Context
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }


    }
}
