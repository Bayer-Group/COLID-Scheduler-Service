﻿using System;
using COLID.Scheduler.Services.Configuration;
using COLID.Scheduler.Services.Implementation;
using COLID.Scheduler.Services.Interface;
using COLID.Scheduler.Services.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace COLID.Scheduler.Services
{
    public static class ServicesModule
    {
        /// <summary>
        /// This will register all the supported functionality by Repositories module.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object for registration.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> object for registration.</param>
        public static IServiceCollection AddServicesModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppDataServiceTokenOptions>(configuration.GetSection("AppDataServiceTokenOptions"));
            services.Configure<ReportingServiceTokenOptions>(configuration.GetSection("ReportingServiceTokenOptions"));

            var smtp = configuration.GetSection("SmtpOptions");
            services.Configure<SmtpOptions>(smtp);

            Console.WriteLine("SMTP Configuration:");
            Console.WriteLine($"- Server/Port: {smtp["Server"]}:{smtp["Port"]}");
            Console.WriteLine($"- SSL enabled: {smtp["EnableSsl"]}");
            Console.WriteLine($"- Sender: {smtp["Sender"]}");
            Console.WriteLine($"- User: {smtp["User"]}");

            services.AddTransient<IRemoteAppDataService, RemoteAppDataService>();
            services.AddTransient<IRemoteReportingService, RemoteReportingService>();
            services.AddTransient<IMailService, MailService>();

            return services;
        }
    }
}
