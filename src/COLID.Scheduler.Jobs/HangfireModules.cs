using System;
using System.Threading;
using COLID.Scheduler.Common.Constants;
using COLID.Scheduler.Jobs.Filter;
using COLID.Scheduler.Jobs.Interface;
using COLID.SchedulerService.Jobs.Implementation;
using COLID.SchedulerService.Jobs.Interface;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace COLID.SchedulerService.Hangfire
{
    public static class HangfireModules
    {
        /// <summary>
        /// This will register all job interfaces and implementations used by Hangfire.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object for registration</param>
        public static IServiceCollection RegisterJobs(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IMessageDeletionJob, MessageDeletionJob>();
            services.AddTransient<IMessageMailingJob, MessageMailingJob>();
            services.AddTransient<IStoredQueriesExecutionJob, StoredQueriesExecutionJob>();
            services.AddTransient<IEntryChangedNotificationJob, EntryChangedNotificationJob>();
            services.AddTransient<IUserInvalidNotificationJob, UserInvalidNotificationJob>();

            return services;
        }

        /// <summary>
        /// This will register Hangfire and it´s database connection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object for registration</param>
        public static IServiceCollection RegisterHangfire(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO: extract to separate Startup.cs classes
            // Use an in-memory store OR a mysql database
            if (config.GetValue<bool>("UseHangfireMemoryStorage"))
            {
                services.AddHangfire(configuration => configuration
                  .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseStorage(new MemoryStorage(new MemoryStorageOptions
                  {
                      JobExpirationCheckInterval = TimeSpan.FromHours(1),
                      CountersAggregateInterval = TimeSpan.FromMinutes(5),
                      FetchNextJobTimeout = TimeSpan.FromMinutes(5)
                  })));
            }
            else
            {
                services.AddHangfire(configuration => configuration
                  .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseStorage(new MySqlStorage(config.GetConnectionString("MySQLConnection").
                                                           Replace("{DB_USER}", config.GetValue<string>("Database:User")).
                                                           Replace("{DB_PASSWORD}", config.GetValue<string>("Database:Password")),
                                             new MySqlStorageOptions
                                             {
                                                 TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                                                 QueuePollInterval = TimeSpan.FromSeconds(15),
                                                 JobExpirationCheckInterval = TimeSpan.FromHours(1),
                                                 CountersAggregateInterval = TimeSpan.FromMinutes(5),
                                                 PrepareSchemaIfNecessary = true,
                                                 DashboardJobListLimit = 5000,
                                                 TransactionTimeout = TimeSpan.FromMinutes(1),
                                                 TablesPrefix = "hangfire_"
                                             })));
            }

            return services;
        }

        /// <summary>
        /// This will setup the Hangfire server which processes the jobs but also the dashboard (UI).
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> object for configuration</param>
        public static IApplicationBuilder SetupHangfireServer(this IApplicationBuilder app, IConfiguration config)
        {
            // TODO ck: extract WorkerCount to configuration
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Queues = new[] { Queue.Alpha, Queue.Beta, Queue.Default },
                WorkerCount = 5,
                CancellationCheckInterval = TimeSpan.FromSeconds(20)
            });

            // Set dashboard authorization and read-only (only on prod)
            DashboardOptions dOptions = new DashboardOptions();
            dOptions.Authorization = new[] { new DashboardAuthorizationFilter() };

            if (config.GetValue<string>("EnvironmentLabel").Equals("Production"))
            {
                dOptions.IsReadOnlyFunc = context => true;
            }

            app.UseHangfireDashboard("/hangfire", dOptions);

            return app;
        }

        /// <summary>
        /// This will setup the Hangfire jobs which will be processed.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> object for configuration</param>
        public static IApplicationBuilder SetupHangfireJobs(this IApplicationBuilder app, IConfiguration config)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<IJob>>();
            logger.LogInformation("Adding recurring jobs for Hangfire");

            var messageMailingJobConfig = config.GetValue<string>("CronJobConfig:MessageMailingJob");
            logger.LogInformation("CronJob config for MessageMailingJob: {messageMailingJobConfig}", messageMailingJobConfig);
            RecurringJob.AddOrUpdate<IMessageMailingJob>(nameof(MessageMailingJob),
                job => job.ExecuteAsync(CancellationToken.None), messageMailingJobConfig,
                TimeZoneInfo.Local,
                Queue.Alpha);

            var userInvalidNotificationJobConfig = config.GetValue<string>("CronJobConfig:UserInvalidNotificationJob");
            logger.LogInformation("CronJob config for UserInvalidNotificationJob: {userInvalidNotificationJobConfig}", userInvalidNotificationJobConfig);
            RecurringJob.AddOrUpdate<IUserInvalidNotificationJob>(nameof(UserInvalidNotificationJob),
                job => job.ExecuteAsync(CancellationToken.None),
                config.GetValue<string>("CronJobConfig:UserInvalidNotificationJob"),
                TimeZoneInfo.Local,
                Queue.Alpha);

            /* NOT YET ACTIVE
            RecurringJob.AddOrUpdate<IEntryChangedNotificationJob>(nameof(EntryChangedNotificationJob),
                job => job.ExecuteAsync(CancellationToken.None),
                config.GetValue<string>("CronJobConfig:EntryChangedNotificationJob"),
                TimeZoneInfo.Local,
                Queue.Alpha);

            RecurringJob.AddOrUpdate<IStoredQueriesExecutionJob>(nameof(StoredQueriesExecutionJob),
                job => job.ExecuteAsync(CancellationToken.None),
                config.GetValue<string>("CronJobConfig:StoredQueriesExecutionJob"),
                TimeZoneInfo.Local,
                Queue.Alpha);

                RecurringJob.AddOrUpdate<IMessageDeletionJob>(nameof(MessageDeletionJob),
                    job => job.ExecuteAsync(CancellationToken.None),
                    config.GetValue<string>("CronJobConfig:MessageDeletionJob"),
                    TimeZoneInfo.Local,
                    Queue.Alpha);
            */
            RecurringJob.RemoveIfExists(nameof(EntryChangedNotificationJob));
            RecurringJob.RemoveIfExists(nameof(MessageDeletionJob));
            RecurringJob.RemoveIfExists(nameof(StoredQueriesExecutionJob));

            return app;
        }
    }
}
