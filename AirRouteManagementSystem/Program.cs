using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace AirRouteManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===============================
            // Add services
            // ===============================

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // ===============================
            // Database
            // ===============================
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // ===============================
            // Dependency Injection
            // ===============================
            builder.Services.RegisterConfig();

            // ===============================
            // Identity
            // ===============================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            // ===============================
            // Authentication (JWT + Google)
            // ===============================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)
                    )
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId =
                    builder.Configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret =
                    builder.Configuration["Authentication:Google:ClientSecret"]!;
            }).AddFacebook(options =>
            {
                options.AppId =
                    builder.Configuration["Authentication:Facebook:AppId"]!;
                options.AppSecret =
                    builder.Configuration["Authentication:Facebook:AppSecret"]!;
            });

            // ===============================
            // Build app
            // ===============================
            var app = builder.Build();

            // ===============================
            // DB Initialize
            // ===============================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var initializer = services.GetRequiredService<IDBInitializar>();
                    initializer.Initialiaze();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error while initializing database");
                }
            }

            // ===============================
            // Middleware
            // ===============================
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
