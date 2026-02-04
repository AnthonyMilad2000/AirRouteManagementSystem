using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Repository;
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
            services.AddScoped<IRepository<Aircraft>, Repository<Aircraft>>();
            services.AddScoped<IRepository<Airport>, Repository<Airport>>();
            services.AddScoped<IRepository<Flight>, Repository<Flight>>();
            services.AddScoped<IRepository<FlightPrice>, Repository<FlightPrice>>();
            services.AddScoped<IAircraftSubImagesRepository, AircraftSubImageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorks>();

            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();

            // Service
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IAccountServices, AccountService>();
            services.AddHttpClient();

            //Open AI
            services.AddHttpClient<IOpenAiService, OpenAiService>();
        }
    }
}
