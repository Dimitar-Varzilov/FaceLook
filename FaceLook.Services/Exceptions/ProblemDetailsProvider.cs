using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FaceLook.Services.Exceptions
{
    /// <summary>
    /// Provides helpers for creating standardized Problem Details responses (RFC 7807).
    /// https://tools.ietf.org/html/rfc7807
    /// </summary>
    public static class ProblemDetailsProvider
    {
        private const string ErrorBaseUrl = "https://yourapi.example.com/errors";

        /// <summary>
        /// Creates a ProblemDetails response based on exception type and status code.
        /// </summary>
        public static ProblemDetails CreateProblemDetails(Exception? exception, int statusCode, string? instance = null)
        {
            string errorType = GetErrorType(statusCode);
            string title = GetErrorTitle(statusCode);
            string detail = exception?.Message ?? GetErrorDetail(statusCode);

            return new ProblemDetails
            {
                Type = $"{ErrorBaseUrl}/{errorType}",
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = instance
            };
        }

        /// <summary>
        /// Gets the error type slug for the Problem Details type URI.
        /// </summary>
        private static string GetErrorType(int statusCode)
        {
            return statusCode switch
            {
                (int)HttpStatusCode.BadRequest => "validation-failed",
                (int)HttpStatusCode.Forbidden => "forbidden",
                (int)HttpStatusCode.NotFound => "not-found",
                (int)HttpStatusCode.Conflict => "conflict",
                422 => "unprocessable-entity",
                (int)HttpStatusCode.InternalServerError => "internal-server-error",
                (int)HttpStatusCode.ServiceUnavailable => "service-unavailable",
                _ => "error"
            };
        }

        /// <summary>
        /// Gets the human-readable title for the Problem Details.
        /// </summary>
        private static string GetErrorTitle(int statusCode)
        {
            return statusCode switch
            {
                (int)HttpStatusCode.BadRequest => "Bad Request",
                (int)HttpStatusCode.Forbidden => "Access Forbidden",
                (int)HttpStatusCode.NotFound => "Resource Not Found",
                (int)HttpStatusCode.Conflict => "Conflict",
                422 => "Unprocessable Entity",
                (int)HttpStatusCode.InternalServerError => "Internal Server Error",
                (int)HttpStatusCode.ServiceUnavailable => "Service Unavailable",
                _ => "An Error Occurred"
            };
        }

        /// <summary>
        /// Gets a default detail message for the status code when no exception is provided.
        /// </summary>
        private static string GetErrorDetail(int statusCode)
        {
            return statusCode switch
            {
                (int)HttpStatusCode.BadRequest => "The request was invalid or malformed.",
                (int)HttpStatusCode.Forbidden => "You do not have permission to access this resource.",
                (int)HttpStatusCode.NotFound => "The requested resource could not be found.",
                (int)HttpStatusCode.Conflict => "The request conflicts with the current state.",
                422 => "The request could not be processed.",
                (int)HttpStatusCode.InternalServerError => "An unexpected error occurred on the server.",
                (int)HttpStatusCode.ServiceUnavailable => "The service is currently unavailable.",
                _ => "An error occurred while processing your request."
            };
        }
    }
}