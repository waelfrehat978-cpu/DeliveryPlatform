using System.Net;
using DeliveryPlatform.Service.Exceptions;

namespace DeliveryPlatform.Api.Middleware
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
                Console.WriteLine(ex.ToString());
                if (context.Response.HasStarted)
                    throw;
                var statusCode = ex switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    ConflictException => (int)HttpStatusCode.Conflict,
                    BadHttpRequestException => (int)HttpStatusCode.BadRequest,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    BusinessRuleException => 422,
                    _ => (int)HttpStatusCode.InternalServerError
                };
                var message = statusCode == 500 ? "An unexpected error occurred." : ex.Message;
                if(statusCode == 500)
                {
                    _logger.LogError(
                        ex, "Unhandled exception. CorrelationId:{CorrelationId}",context.TraceIdentifier
                        );
                }
                else
                {
                    _logger.LogWarning(
                        "Request failed eith status {StatusCode}:{Message}",
                        statusCode,ex.Message
                        );
                }
                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = statusCode,
                    Message=message,
                    CorrelationId=context.TraceIdentifier
                });

            }
        }
    }
}
