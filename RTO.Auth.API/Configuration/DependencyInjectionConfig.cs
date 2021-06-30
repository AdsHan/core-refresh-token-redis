using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RTO.Auth.API.Data;
using RTO.Auth.API.Entities;
using RTO.Auth.API.Extensions;
using RTO.Auth.API.Service;

namespace RTO.Auth.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyConfiguration(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {

            // Usando com banco de dados em memória
            services.AddDbContext<AuthDbContext>(options => options.UseInMemoryDatabase("AuthDB"));

            services.AddIdentity<UserModel, IdentityRole>(options =>
                {
                    // Configurações de senha
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 2;
                    options.Password.RequiredUniqueChars = 0;
                })
                .AddErrorDescriber<IdentityPortugues>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<AuthService>();

            services.AddTransient<UserInitializerService>();

            return services;
        }

    }
}