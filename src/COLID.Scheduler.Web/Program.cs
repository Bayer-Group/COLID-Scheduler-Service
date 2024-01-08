using COLID.Common.Logger;
using COLID.Exception;
using COLID.Identity;
using COLID.Scheduler.Services;
using COLID.SchedulerService.Hangfire;
using COLID.StatisticsLog;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
builder.Services.AddDefaultCorrelationId();
builder.Services.AddCorrelationIdLogger();
builder.Services.AddLogging();
builder.Services.AddServicesModule(configuration);
builder.Services.RegisterJobs();
builder.Services.RegisterHangfire(configuration);
builder.Services.AddIdentityModule(configuration);
builder.Services.AddStatisticsLogModule(configuration);

var app = builder.Build();

app.UseCorrelationId();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health");
});
app.SetupHangfireServer(configuration);
app.SetupHangfireJobs(configuration);
app.UseExceptionMiddleware();

app.Run();

