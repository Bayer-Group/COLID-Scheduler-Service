using COLID.Identity;
using COLID.Scheduler.ExceptionMiddleware;
using COLID.Scheduler.Services;
using COLID.SchedulerService.Hangfire;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using COLID.StatisticsLog;
using COLID.Common.Logger;

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
            services.AddDefaultCorrelationId();
            services.AddCorrelationIdLogger();

            services.AddLogging();
            services.AddServicesModule(Configuration);

            services.RegisterJobs();
            services.RegisterHangfire(Configuration);

            services.AddIdentityModule(Configuration);

            services.AddStatisticsLogModule(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCorrelationId();
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
