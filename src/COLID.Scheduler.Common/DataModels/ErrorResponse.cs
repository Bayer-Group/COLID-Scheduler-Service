﻿using System.Net;

namespace COLID.SchedulerService.Common.DataModels
{
    public class ErrorResponse
    {
        public HttpStatusCode Code { get; set; }

        public string Message { get; set; }
    }
}
