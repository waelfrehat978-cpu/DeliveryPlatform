
using DeliveryPlatform.Service.Exceptions;

namespace DeliveryPlatform.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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
                if (context.Response.HasStarted)
                    throw;

                var statusCode = ex switch
                {
                    AppException appException => appException.StatusCode,
                    BadHttpRequestException => 400,
                    _ => 500
                };

                var message = statusCode == 500
                    ? "An unexpected error occurred."
                    : ex.Message;

                if (statusCode == 500)
                {
                    _logger.LogError(
                        ex,
                        "Unhandled exception. CorrelationId: {CorrelationId}",
                        context.TraceIdentifier
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "Request failed with status {StatusCode}: {Message}",
                        statusCode,
                        ex.Message
                    );
                }

                var errors = (ex as ValidationException)?.Errors;

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = statusCode,
                    Message = message,
                    Errors = errors,
                    CorrelationId = context.TraceIdentifier
                });
            }
        }
    }
}
