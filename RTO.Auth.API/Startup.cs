using DSC.Auth.API.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RTO.Auth.API.Configuration;
using RTO.Auth.API.Service;

namespace RTO.Auth.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApiConfiguration(Configuration);

            services.AddDependencyConfiguration(Configuration);

            services.AddSwaggerConfiguration();

            services.AddRedisConfiguration(Configuration);

            services.AddSettingsConfiguration(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserInitializerService userInitializerService)
        {

            app.UseApiConfiguration(env, userInitializerService);

            app.UseSwaggerConfiguration();

        }
    }
}
