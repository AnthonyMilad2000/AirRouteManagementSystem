
using AirRouteManagementSystem.DateAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace AirRouteManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // database configuration
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
               options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

            //Dependency Injection
            builder.Services.RegisterConfig();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
