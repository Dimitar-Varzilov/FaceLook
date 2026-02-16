namespace FaceLook.Services.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string sender, string message);
    }
}
