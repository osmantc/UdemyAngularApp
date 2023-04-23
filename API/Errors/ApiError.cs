using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiError
    {
        public int StatusCode { get; }
        public string Message { get; }
        public string? Details { get; }
        public ApiError(int statusCode, string message, string? details)
        {
            this.Details = details;
            this.Message = message;
            this.StatusCode = statusCode;
        }
    }
}