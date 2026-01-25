using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AirRouteManagementSystem
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services) 
        {
           // Email Sender
            services.AddTransient<IEmailSender,EmailSender>();
        }
    }
}
