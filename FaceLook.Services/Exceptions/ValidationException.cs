using System.Net;

namespace FaceLook.Services.Exceptions
{
    public class ValidationException(string? message = null) : Exception(message), IHttpStatusCodeCarrier
    {
        public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
