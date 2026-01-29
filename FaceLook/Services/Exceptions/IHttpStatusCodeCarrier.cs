using System.Net;

namespace FaceLook.Services.Exceptions
{
    // <summary>
    /// Defines a contract for exceptions that carry HTTP status code information.
    /// </summary>
    public interface IHttpStatusCodeCarrier
    {
        HttpStatusCode StatusCode { get; }
    }
}