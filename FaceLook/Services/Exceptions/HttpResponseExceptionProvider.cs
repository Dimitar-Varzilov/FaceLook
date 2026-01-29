using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FaceLook.Services.Exceptions
{
    /// <summary>
    /// Provides helpers for HTTP response handling and exception conversion.
    /// </summary>
    public static class HttpResponseExceptionProvider
    {
        /// <summary>
        /// Gets the HTTP status code for a given exception.
        /// </summary>
        public static int GetStatusCode(Exception? ex)
        {
            if (ex == null)
                return (int)HttpStatusCode.InternalServerError;

            // Check if exception carries HTTP metadata
            if (ex is IHttpStatusCodeCarrier carrier)
                return (int)carrier.StatusCode;

            return (int)HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Creates a ProblemDetails response for the given exception.
        /// </summary>
        public static ProblemDetails CreateProblemDetails(Exception exception, string? instance = null)
        {
            int statusCode = GetStatusCode(exception);
            return ProblemDetailsProvider.CreateProblemDetails(exception, statusCode, instance);
        }
    }
}