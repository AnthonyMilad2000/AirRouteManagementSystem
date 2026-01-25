using Microsoft.EntityFrameworkCore;

namespace AirRouteManagementSystem.DateAccess.Context
{
    public class ApplicationDBContext :DbContext
    {
        ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }


    }
}
