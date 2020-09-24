using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.SchedulerService.Common.DataModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace COLID.SchedulerService.ExceptionMiddleware
{
    /// <summary>
    /// Central error/exception handler middleware
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="httpContext">The context.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception);
            }
        }

        /// <summary>
        /// Handles all exceptions that are thrown by COLID and could not be treated.
        /// </summary>
        /// <param name="httpContext">the context of request.</param>
        /// <param name="exception">The untreated exception.</param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext httpContext, System.Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            /*
            Mapping of http return code [...]
            */

            httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            httpContext.Response.StatusCode = (int)code;

            var responseBody = new ErrorResponse
            {
                Code = code,
                Message = exception.Message
            };

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(responseBody, _jsonSerializerSettings));
        }
    }
}
