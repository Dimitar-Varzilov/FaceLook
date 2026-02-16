using FaceLook.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FaceLook.Services.Middlewares
{
    /// <summary>
    /// Factory-based middleware for global exception handling in ASP.NET Core.
    /// </summary>
    public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
    {
        /// <inheritdoc/>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception with request context
            string method = context.Request.Method;
            string path = context.Request.Path.ToString();
            logger.LogError(exception, "{Method} {Path} - Unhandled exception", method, path);

            // Get status code and create Problem Details response
            int statusCode = HttpResponseExceptionProvider.GetStatusCode(exception);
            var problemDetails = HttpResponseExceptionProvider.CreateProblemDetails(exception, path);

            // Write response
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
