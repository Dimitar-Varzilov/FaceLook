using FaceLook.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FaceLook.Services
{
    /// <summary>
    /// Interface for use in dependency injection
    /// </summary>
    public interface IFaceLookHubContext : IHubContext<ChatHub, IChatClient>
    {
    }
}
