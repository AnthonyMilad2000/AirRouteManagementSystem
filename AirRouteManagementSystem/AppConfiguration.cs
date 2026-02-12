using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Model.Customer;
using AirRouteManagementSystem.Repository;
using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.Services;
using AirRouteManagementSystem.SignalR;
using AirRouteManagementSystem.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;

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
            services.AddScoped<IRepository<ApplicationUser>, Repository<ApplicationUser>>();
            services.AddScoped<IAircraftSubImagesRepository, AircraftSubImageRepository>();
            services.AddScoped<IFlightPriceRepository, FlightPriceRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorks>();

            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IRepository<Booking>, Repository<Booking>>();
            services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();

            // Service
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IAccountServices, AccountService>();
            services.AddHttpClient();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddSignalR();
            //Open AI
            services.AddHttpClient<IOpenAiService, OpenAiService>();
        }
    }
}
