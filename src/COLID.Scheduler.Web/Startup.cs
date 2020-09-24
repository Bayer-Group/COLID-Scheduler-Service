using COLID.Identity;
using COLID.Scheduler.ExceptionMiddleware;
using COLID.Scheduler.Services;
using COLID.SchedulerService.Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder().AddConfiguration(configuration).Build();
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddHealthChecks();

            services.AddLogging();
            services.AddServicesModule(Configuration);

            services.RegisterJobs();
            services.RegisterHangfire(Configuration);

            services.AddIdentityModule(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });

            app.SetupHangfireServer(Configuration);
            app.SetupHangfireJobs(Configuration);

            app.UseExceptionMiddleware();
        }
    }
}
