using System;
using COLID.SchedulerService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace COLID.AppDataService.FunctionalTests
{
    public class FunctionTestsFixture : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "appsettings.Testing.json");
                conf.AddUserSecrets<Startup>();
            });

            builder.ConfigureServices(services =>
            {
            });
        }
    }
}
