using AirRouteManagementSystem.Reposatory;
using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.Services;
using AirRouteManagementSystem.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AirRouteManagementSystem
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services)
        {
            // Email Sender
            services.AddTransient<IEmailSender, EmailSender>();

            // DBInitializar
            services.AddScoped<IDBInitializar, DBInitializar>();

            // Reposatoey
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();

            // Service
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IAccountServices, AccountService>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddHttpClient();

            //Open AI
            services.AddHttpClient<IOpenAiService, OpenAiService>();

        }
    }
}
