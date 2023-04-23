using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        public ILogger<ErrorMiddleware> _logger { get; }
        private readonly IHostEnvironment _environment;

        public ErrorMiddleware(RequestDelegate next, IHostEnvironment environment, ILogger<ErrorMiddleware> logger)
        {
            this._environment = environment;
            this._logger = logger;
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                ApiError apiError = new ApiError((int)StatusCodes.Status500InternalServerError, ex.Message + " error_tarik", _environment.IsDevelopment() ? ex.StackTrace?.ToString() : "Internal Server Error stack_tarik");

                JsonSerializerOptions jsonOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                await context.Response.WriteAsync(JsonSerializer.Serialize(apiError, jsonOptions));
            }
        }

    }
}