using System.Text.Json;
using System.Net;

namespace SafeEscape.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;
            var statusCode = HttpStatusCode.InternalServerError;

            if (exception is KeyNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (exception is UnauthorizedAccessException unauthorizedException)
            {
                statusCode = HttpStatusCode.Unauthorized;

                if (unauthorizedException.Message == "User is banned")
                {
                    statusCode = HttpStatusCode.Forbidden;
                }
            }


            response.StatusCode = (int)statusCode;

            var result = new { message = exception.Message };
            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
