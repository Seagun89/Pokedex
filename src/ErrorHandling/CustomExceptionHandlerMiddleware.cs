namespace API.ErrorHandling
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // needs to be awaited to catch exceptions that occur in the next middleware or request processing pipeline
            } 
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");

            var message = "";
            var stackTrace = "";

            switch (ex)
            {
                case KeyNotFoundException: 
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    stackTrace = ex.StackTrace;
                    break;
                case ArgumentNullException:
                case ArgumentOutOfRangeException:
                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    stackTrace = ex.StackTrace;
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    message = ex.Message;
                    break;
            }

            var errorMessage = new {message, stackTrace};
            await context.Response.WriteAsJsonAsync(errorMessage);
        }
    }
}