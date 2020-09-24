namespace COLID.Scheduler.ExceptionMiddleware
{
    using Microsoft.AspNetCore.Builder;

    public static class ExceptionMiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<COLID.SchedulerService.ExceptionMiddleware.ExceptionMiddleware>();
        }
    }
}
