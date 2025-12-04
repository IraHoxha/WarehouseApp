using System.Text.Json;
using warehouseapp.Exceptions;

namespace warehouseapp.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/json";

                int statusCode = ex switch
                {
                    NotFoundException   => StatusCodes.Status404NotFound,
                    ValidationException => StatusCodes.Status400BadRequest,
                    _                   => StatusCodes.Status500InternalServerError
                };

                var response = new
                {
                    status = statusCode,
                    message = ex.Message
                };

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
