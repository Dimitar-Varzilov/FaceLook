using System.Net;

namespace FaceLook.Services.Exceptions
{
    public class ResourceNotFoundException(string? message = null) : Exception(message), IHttpStatusCodeCarrier
    {
        public HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}
